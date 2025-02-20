from typing import List, Dict, Any, Callable
import inspect
from functools import wraps
from google.protobuf.descriptor import ServiceDescriptor, MethodDescriptor, Descriptor, FieldDescriptor
import grpc
from langchain.tools import Tool
import NodeTools_pb2
import NodeTools_pb2_grpc
from google.protobuf import descriptor_pool
from google.protobuf import descriptor

# importing the defined extensions from NodeTools_pb2
# so that we can reference them below
method_description_ext = NodeTools_pb2.method_description
field_description_ext = NodeTools_pb2.field_description

def extract_proto_docs() -> Dict[str, Dict[str, str]]:
    """
    extracts documentation from proto file options using the descriptors
    returns a dict of method names to their documentation
    """
    service_descriptor: ServiceDescriptor = NodeTools_pb2.DESCRIPTOR.services_by_name['NodeToolService']
    docs = {}

    for method in service_descriptor.methods:
        method_name = method.name
        # get input message descriptor
        input_type = method.input_type
        # get output message descriptor
        output_type = method.output_type

        print(method.name)
        # get documentation from the method options
        method_desc = method.GetOptions().Extensions[NodeTools_pb2.method_description]

        # build parameter docs from field options
        param_docs = {}
        for field in input_type.fields:
            field_name = field.name
            field_desc = field.GetOptions().Extensions[NodeTools_pb2.field_description]
            param_docs[field_name] = field_desc

        docs[method_name] = {
            'description': method_desc,
            'parameters': param_docs,
            'return_type': output_type.name
        }

    return docs

def create_grpc_tool(stub: Any, method_name: str, docs: Dict[str, Any]) -> Tool:
    """creates a langchain tool for a single grpc method"""

    # get the request and response message types
    request_type = getattr(NodeTools_pb2, f"{method_name}Request", None)
    if request_type is None and method_name in ['GetAllNodes', 'GetSelectedNodes', 'GetNodeTypes']:
        request_type = NodeTools_pb2.Empty

    # create the function that will handle the grpc call
    def grpc_tool(**kwargs) -> str:
        try:
            # create request object
            if request_type == NodeTools_pb2.Empty:
                request = request_type()
            else:
                request = request_type(**kwargs)

            # make grpc call
            response = getattr(stub, method_name)(request)

            # handle different response types
            if hasattr(response, 'result'):
                return response.result
            elif hasattr(response, 'nodes'):
                return response.nodes
            elif hasattr(response, 'types'):
                return response.types
            return str(response)

        except grpc.RpcError as e:
            return f"gRPC error: {str(e)}"

    # build tool description from docs
    description = docs.get('description', '')
    param_docs = docs.get('parameters', {})
    param_desc = "\nParameters:\n" + "\n".join(
        f"- {name}: {desc}" for name, desc in param_docs.items()
    ) if param_docs else ""

    full_description = f"{description}{param_desc}"

    # create and return the tool
    return Tool(
        name=method_name,
        description=full_description,
        func=grpc_tool
    )

def generate_grpc_tools(channel: grpc.Channel) -> List[Tool]:
    """
    generates a list of langchain tools based on the NodeToolService definition
    """
    stub = NodeTools_pb2_grpc.NodeToolServiceStub(channel)

    # get descriptor for NodeToolService
    service_descriptor: ServiceDescriptor = NodeTools_pb2.DESCRIPTOR.services_by_name["NodeToolService"]

    tools = []
    for method_desc in service_descriptor.methods:
        tool = create_tool_for_method(method_desc, stub)
        tools.append(tool)

    return tools

def create_tool_for_method(method_desc: descriptor.MethodDescriptor, stub: Any) -> Tool:
    """
    creates a single langchain tool for the given method descriptor
    """
    method_name = method_desc.name

    if method_desc.GetOptions().HasExtension(method_description_ext):
        method_doc = method_desc.GetOptions().Extensions[method_description_ext]
    else:
        method_doc = ""

    input_type = method_desc.input_type
    output_type = method_desc.output_type
    fields = input_type.fields

    parameter_lines = []
    for f in fields:
        if f.GetOptions().HasExtension(field_description_ext):
            field_doc = f.GetOptions().Extensions[field_description_ext]
        else:
            field_doc = "(no field description)"

        parameter_lines.append(f"- **{f.name}**: {field_doc}")

    if parameter_lines:
        param_docs = "\nparameters:\n" + "\n".join(parameter_lines)
    else:
        param_docs = "\n(no parameters)"

    full_description = f"{method_doc}{param_docs}"

    # if the method has no fields, produce a structured tool with an empty pydantic model
    if not fields:
        from langchain.tools import StructuredTool
        from pydantic import BaseModel

        # define an empty schema
        class NoParams(BaseModel):
            pass

        def grpc_tool(args: NoParams = None) -> str:
            # if args is missing, use an empty instance
            if args is None:
                args = NoParams()
            try:
                request = NodeTools_pb2.Empty()
                response = getattr(stub, method_name)(request)
                return extract_response_str(response)
            except grpc.RpcError as e:
                return f"gRPC error: {str(e)}"

        return StructuredTool(
            name=method_name,
            description=full_description,
            func=grpc_tool,
            args_schema=NoParams
        )
    else:
        # if the method has fields, we handle it as before
        func = build_dynamic_function(method_name, stub, fields, full_description)
        return Tool.from_function(
            name=method_name,
            func=func,
            description=full_description,
        )

def extract_response_str(response: Any) -> str:
    """
    attempts to extract a string from different response types
    """
    if hasattr(response, "result"):
        return response.result
    elif hasattr(response, "nodes"):
        return str(response.nodes)
    elif hasattr(response, "types"):
        return str(response.types)
    else:
        return str(response)

def build_dynamic_function(method_name: str, stub: Any, fields: List[FieldDescriptor], doc: str):
    """
    builds a python function with an explicit parameter for each proto field
    """
    import inspect
    from inspect import Parameter, Signature

    def dynamic_grpc_call(**kwargs):
        # attempt to find a matching request message, e.g. "CreateNodeRequest"
        request_message = getattr(NodeTools_pb2, f"{method_name}Request", None)
        if request_message is None:
            # fallback: use Empty if the request message is not found
            request_instance = NodeTools_pb2.Empty()
        else:
            request_instance = request_message(**kwargs)

        try:
            response = getattr(stub, method_name)(request_instance)
            return extract_response_str(response)
        except grpc.RpcError as e:
            return f"gRPC error: {str(e)}"

    dynamic_grpc_call.__name__ = method_name
    dynamic_grpc_call.__doc__ = doc

    # build a custom signature with the proto field names, each as a keyword-only parameter
    parameters = []
    for f in fields:
        parameters.append(Parameter(
            name=f.name,
            kind=Parameter.KEYWORD_ONLY,
            default=None
        ))
    sig = Signature(parameters, return_annotation=str)

    dynamic_grpc_call.__signature__ = sig  # type: ignore

    return dynamic_grpc_call

def get_tools():
    # create channel
    channel = grpc.insecure_channel('localhost:5001')
    # generate tools
    tools = generate_grpc_tools(channel)

    for tool in tools:
        print(f"\nTool: {tool.name}")
        print(f"Description: {tool.description}")
        print(f"Parameters: {inspect.signature(tool.func)}")
        #print(f"Signature: {tool.func.__signature__}\n")
    return tools

if __name__ == "__main__":
    # example usage
    channel = grpc.insecure_channel("localhost:5001")
    tools = generate_grpc_tools(channel)
    for t in tools:
        print(f"\nTool name: {t.name}")
        print(f"Description:\n{t.description}")
        print(f"Signature: {t.func.__signature__}\n")