#!/usr/bin/env python
"""
this script inspects the NodeToolService defined in NodeTools_pb2 and
dynamically generates a python script containing langchain tool functions.
"""

import os
from google.protobuf.descriptor import FieldDescriptor
import NodeTools_pb2

# try to load proto extensions for documentation
try:
    method_description_ext = NodeTools_pb2.method_description
    field_description_ext = NodeTools_pb2.field_description
except Exception:
    method_description_ext = None
    field_description_ext = None

def main():
    # get service descriptor
    service = NodeTools_pb2.DESCRIPTOR.services_by_name["NodeToolService"]

    lines = []
    lines.append("#!/usr/bin/env python")
    lines.append('"""')
    lines.append("this file is dynamically generated; do not edit directly.")
    lines.append('"""')
    lines.append("")
    lines.append("import grpc")
    lines.append("from langchain.tools import tool")
    lines.append("from typing import Annotated, List")
    lines.append("import NodeTools_pb2")
    lines.append("import NodeTools_pb2_grpc")
    lines.append("")
    lines.append("# create channel and stub")
    lines.append("channel = grpc.insecure_channel('localhost:5001')")
    lines.append("stub = NodeTools_pb2_grpc.NodeToolServiceStub(channel)")
    lines.append("")

    tool_names = []

    # generate a tool function for each rpc method
    for method in service.methods:
        method_name = method.name
        input_descriptor = method.input_type
        output_descriptor = method.output_type

        # determine return type from output message
        ret_type = "str"  # default for OperationResponse
        if output_descriptor.name == "NodesResponse":
            ret_type = "List[str]"
        elif output_descriptor.name == "NodeTypesResponse":
            ret_type = "List[str]"
        elif output_descriptor.name == "PropertiesResponse":
            ret_type = "dict[str, str]"
        elif output_descriptor.name == "AnimationNamesResponse":
            ret_type = "List[str]"

        # get method description
        method_doc = method.GetOptions().Extensions[method_description_ext] if method_description_ext else ""
        if not method_doc:
            method_doc = f"calls the {method_name} rpc method"

        # build function parameters with Annotated types
        args_list = []
        if input_descriptor.name != "Empty" and len(input_descriptor.fields) > 0:
            for field in input_descriptor.fields:
                field_doc = field.GetOptions().Extensions[field_description_ext] if field_description_ext else ""
                # determine the python type for the field
                if field.type == FieldDescriptor.TYPE_STRING:
                    resolved_type = "str"
                elif field.type in (FieldDescriptor.TYPE_FLOAT, FieldDescriptor.TYPE_DOUBLE):
                    resolved_type = "float"
                elif field.type in (FieldDescriptor.TYPE_INT32, FieldDescriptor.TYPE_INT64, FieldDescriptor.TYPE_UINT32, FieldDescriptor.TYPE_UINT64):
                    resolved_type = "int"
                elif field.type == FieldDescriptor.TYPE_BOOL:
                    resolved_type = "bool"
                else:
                    resolved_type = "str"
                # for repeated fields, use List with the resolved type
                if field.label == FieldDescriptor.LABEL_REPEATED:
                    resolved_annotation = f"List[{resolved_type}]"
                else:
                    resolved_annotation = resolved_type

                args_list.append(f"{field.name}: Annotated[{resolved_annotation}, \"{field_doc}\"]")
        args_str = ",\n    ".join(args_list)

        # generate the function
        lines.append("@tool")
        if args_list:
            lines.append(f"def {method_name}(")
            lines.append(f"    {args_str}")
            lines.append(f") -> {ret_type}:")
        else:
            lines.append(f"def {method_name}() -> {ret_type}:")
        lines.append(f'    """{method_doc}"""')

        # create request object
        if input_descriptor.name == "Empty":
            lines.append("    request = NodeTools_pb2.Empty()")
        else:
            assignments = ", ".join([f"{field.name}={field.name}" for field in input_descriptor.fields])
            lines.append(f"    request = NodeTools_pb2.{input_descriptor.name}({assignments})")

        # make rpc call and return appropriate field
        lines.append(f"    response = stub.{method_name}(request)")
        if output_descriptor.name == "NodesResponse":
            lines.append("    return response.nodes")
        elif output_descriptor.name == "NodeTypesResponse":
            lines.append("    return response.types")
        elif output_descriptor.name == "PropertiesResponse":
            lines.append("    return response.properties")
        elif output_descriptor.name == "AnimationNamesResponse":
            lines.append("    return response.names")
        else:  # OperationResponse
            lines.append("    return response.result")
        lines.append("")
        tool_names.append(method_name)

    # add tools list
    lines.append("tools = [")
    for t in tool_names:
        lines.append(f"    {t},")
    lines.append("]")
    lines.append("")

    # write generated code to file
    output_path = os.path.join(os.path.dirname(__file__), "generated_tools.py")
    with open(output_path, "w") as f:
        f.write("\n".join(lines))
    print(f"generated '{output_path}' successfully.")

if __name__ == "__main__":
    main()