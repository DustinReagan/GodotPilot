# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc
import warnings

import NodeTools_pb2 as NodeTools__pb2

GRPC_GENERATED_VERSION = '1.70.0'
GRPC_VERSION = grpc.__version__
_version_not_supported = False

try:
    from grpc._utilities import first_version_is_lower
    _version_not_supported = first_version_is_lower(GRPC_VERSION, GRPC_GENERATED_VERSION)
except ImportError:
    _version_not_supported = True

if _version_not_supported:
    raise RuntimeError(
        f'The grpc package installed is at version {GRPC_VERSION},'
        + f' but the generated code in NodeTools_pb2_grpc.py depends on'
        + f' grpcio>={GRPC_GENERATED_VERSION}.'
        + f' Please upgrade your grpc module to grpcio>={GRPC_GENERATED_VERSION}'
        + f' or downgrade your generated code using grpcio-tools<={GRPC_VERSION}.'
    )


class NodeToolServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.GetAllNodes = channel.unary_unary(
                '/nodetools.NodeToolService/GetAllNodes',
                request_serializer=NodeTools__pb2.Empty.SerializeToString,
                response_deserializer=NodeTools__pb2.NodesResponse.FromString,
                _registered_method=True)
        self.GetSelectedNodes = channel.unary_unary(
                '/nodetools.NodeToolService/GetSelectedNodes',
                request_serializer=NodeTools__pb2.Empty.SerializeToString,
                response_deserializer=NodeTools__pb2.NodesResponse.FromString,
                _registered_method=True)
        self.GetNodesWithName = channel.unary_unary(
                '/nodetools.NodeToolService/GetNodesWithName',
                request_serializer=NodeTools__pb2.NameRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.NodesResponse.FromString,
                _registered_method=True)
        self.CreateNode = channel.unary_unary(
                '/nodetools.NodeToolService/CreateNode',
                request_serializer=NodeTools__pb2.CreateNodeRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.DeleteNode = channel.unary_unary(
                '/nodetools.NodeToolService/DeleteNode',
                request_serializer=NodeTools__pb2.PathRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.RenameNode = channel.unary_unary(
                '/nodetools.NodeToolService/RenameNode',
                request_serializer=NodeTools__pb2.RenameNodeRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.ChangeNodeType = channel.unary_unary(
                '/nodetools.NodeToolService/ChangeNodeType',
                request_serializer=NodeTools__pb2.ChangeNodeTypeRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.DuplicateNode = channel.unary_unary(
                '/nodetools.NodeToolService/DuplicateNode',
                request_serializer=NodeTools__pb2.DuplicateNodeRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.SetNodeProperty = channel.unary_unary(
                '/nodetools.NodeToolService/SetNodeProperty',
                request_serializer=NodeTools__pb2.SetNodePropertyRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.SelectNodes = channel.unary_unary(
                '/nodetools.NodeToolService/SelectNodes',
                request_serializer=NodeTools__pb2.SelectNodesRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.ReparentNodes = channel.unary_unary(
                '/nodetools.NodeToolService/ReparentNodes',
                request_serializer=NodeTools__pb2.ReparentNodesRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.CopyNodeProperties = channel.unary_unary(
                '/nodetools.NodeToolService/CopyNodeProperties',
                request_serializer=NodeTools__pb2.CopyNodePropertiesRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.GetNodeTypes = channel.unary_unary(
                '/nodetools.NodeToolService/GetNodeTypes',
                request_serializer=NodeTools__pb2.Empty.SerializeToString,
                response_deserializer=NodeTools__pb2.NodeTypesResponse.FromString,
                _registered_method=True)
        self.GetAllNodesByType = channel.unary_unary(
                '/nodetools.NodeToolService/GetAllNodesByType',
                request_serializer=NodeTools__pb2.NodeTypeRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.NodesResponse.FromString,
                _registered_method=True)
        self.GetNodeProperties = channel.unary_unary(
                '/nodetools.NodeToolService/GetNodeProperties',
                request_serializer=NodeTools__pb2.PathRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.PropertiesResponse.FromString,
                _registered_method=True)
        self.CreateAnimation = channel.unary_unary(
                '/nodetools.NodeToolService/CreateAnimation',
                request_serializer=NodeTools__pb2.CreateAnimationRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.DeleteAnimation = channel.unary_unary(
                '/nodetools.NodeToolService/DeleteAnimation',
                request_serializer=NodeTools__pb2.DeleteAnimationRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.SetAnimationSpeed = channel.unary_unary(
                '/nodetools.NodeToolService/SetAnimationSpeed',
                request_serializer=NodeTools__pb2.SetAnimationSpeedRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.RenameAnimation = channel.unary_unary(
                '/nodetools.NodeToolService/RenameAnimation',
                request_serializer=NodeTools__pb2.RenameAnimationRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.OperationResponse.FromString,
                _registered_method=True)
        self.GetAnimationNames = channel.unary_unary(
                '/nodetools.NodeToolService/GetAnimationNames',
                request_serializer=NodeTools__pb2.PathRequest.SerializeToString,
                response_deserializer=NodeTools__pb2.AnimationNamesResponse.FromString,
                _registered_method=True)


class NodeToolServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def GetAllNodes(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetSelectedNodes(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetNodesWithName(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def CreateNode(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DeleteNode(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def RenameNode(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def ChangeNodeType(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DuplicateNode(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def SetNodeProperty(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def SelectNodes(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def ReparentNodes(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def CopyNodeProperties(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetNodeTypes(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetAllNodesByType(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetNodeProperties(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def CreateAnimation(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DeleteAnimation(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def SetAnimationSpeed(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def RenameAnimation(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def GetAnimationNames(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_NodeToolServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'GetAllNodes': grpc.unary_unary_rpc_method_handler(
                    servicer.GetAllNodes,
                    request_deserializer=NodeTools__pb2.Empty.FromString,
                    response_serializer=NodeTools__pb2.NodesResponse.SerializeToString,
            ),
            'GetSelectedNodes': grpc.unary_unary_rpc_method_handler(
                    servicer.GetSelectedNodes,
                    request_deserializer=NodeTools__pb2.Empty.FromString,
                    response_serializer=NodeTools__pb2.NodesResponse.SerializeToString,
            ),
            'GetNodesWithName': grpc.unary_unary_rpc_method_handler(
                    servicer.GetNodesWithName,
                    request_deserializer=NodeTools__pb2.NameRequest.FromString,
                    response_serializer=NodeTools__pb2.NodesResponse.SerializeToString,
            ),
            'CreateNode': grpc.unary_unary_rpc_method_handler(
                    servicer.CreateNode,
                    request_deserializer=NodeTools__pb2.CreateNodeRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'DeleteNode': grpc.unary_unary_rpc_method_handler(
                    servicer.DeleteNode,
                    request_deserializer=NodeTools__pb2.PathRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'RenameNode': grpc.unary_unary_rpc_method_handler(
                    servicer.RenameNode,
                    request_deserializer=NodeTools__pb2.RenameNodeRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'ChangeNodeType': grpc.unary_unary_rpc_method_handler(
                    servicer.ChangeNodeType,
                    request_deserializer=NodeTools__pb2.ChangeNodeTypeRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'DuplicateNode': grpc.unary_unary_rpc_method_handler(
                    servicer.DuplicateNode,
                    request_deserializer=NodeTools__pb2.DuplicateNodeRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'SetNodeProperty': grpc.unary_unary_rpc_method_handler(
                    servicer.SetNodeProperty,
                    request_deserializer=NodeTools__pb2.SetNodePropertyRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'SelectNodes': grpc.unary_unary_rpc_method_handler(
                    servicer.SelectNodes,
                    request_deserializer=NodeTools__pb2.SelectNodesRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'ReparentNodes': grpc.unary_unary_rpc_method_handler(
                    servicer.ReparentNodes,
                    request_deserializer=NodeTools__pb2.ReparentNodesRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'CopyNodeProperties': grpc.unary_unary_rpc_method_handler(
                    servicer.CopyNodeProperties,
                    request_deserializer=NodeTools__pb2.CopyNodePropertiesRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'GetNodeTypes': grpc.unary_unary_rpc_method_handler(
                    servicer.GetNodeTypes,
                    request_deserializer=NodeTools__pb2.Empty.FromString,
                    response_serializer=NodeTools__pb2.NodeTypesResponse.SerializeToString,
            ),
            'GetAllNodesByType': grpc.unary_unary_rpc_method_handler(
                    servicer.GetAllNodesByType,
                    request_deserializer=NodeTools__pb2.NodeTypeRequest.FromString,
                    response_serializer=NodeTools__pb2.NodesResponse.SerializeToString,
            ),
            'GetNodeProperties': grpc.unary_unary_rpc_method_handler(
                    servicer.GetNodeProperties,
                    request_deserializer=NodeTools__pb2.PathRequest.FromString,
                    response_serializer=NodeTools__pb2.PropertiesResponse.SerializeToString,
            ),
            'CreateAnimation': grpc.unary_unary_rpc_method_handler(
                    servicer.CreateAnimation,
                    request_deserializer=NodeTools__pb2.CreateAnimationRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'DeleteAnimation': grpc.unary_unary_rpc_method_handler(
                    servicer.DeleteAnimation,
                    request_deserializer=NodeTools__pb2.DeleteAnimationRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'SetAnimationSpeed': grpc.unary_unary_rpc_method_handler(
                    servicer.SetAnimationSpeed,
                    request_deserializer=NodeTools__pb2.SetAnimationSpeedRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'RenameAnimation': grpc.unary_unary_rpc_method_handler(
                    servicer.RenameAnimation,
                    request_deserializer=NodeTools__pb2.RenameAnimationRequest.FromString,
                    response_serializer=NodeTools__pb2.OperationResponse.SerializeToString,
            ),
            'GetAnimationNames': grpc.unary_unary_rpc_method_handler(
                    servicer.GetAnimationNames,
                    request_deserializer=NodeTools__pb2.PathRequest.FromString,
                    response_serializer=NodeTools__pb2.AnimationNamesResponse.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'nodetools.NodeToolService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))
    server.add_registered_method_handlers('nodetools.NodeToolService', rpc_method_handlers)


 # This class is part of an EXPERIMENTAL API.
class NodeToolService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def GetAllNodes(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetAllNodes',
            NodeTools__pb2.Empty.SerializeToString,
            NodeTools__pb2.NodesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetSelectedNodes(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetSelectedNodes',
            NodeTools__pb2.Empty.SerializeToString,
            NodeTools__pb2.NodesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetNodesWithName(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetNodesWithName',
            NodeTools__pb2.NameRequest.SerializeToString,
            NodeTools__pb2.NodesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def CreateNode(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/CreateNode',
            NodeTools__pb2.CreateNodeRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DeleteNode(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/DeleteNode',
            NodeTools__pb2.PathRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def RenameNode(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/RenameNode',
            NodeTools__pb2.RenameNodeRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def ChangeNodeType(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/ChangeNodeType',
            NodeTools__pb2.ChangeNodeTypeRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DuplicateNode(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/DuplicateNode',
            NodeTools__pb2.DuplicateNodeRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def SetNodeProperty(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/SetNodeProperty',
            NodeTools__pb2.SetNodePropertyRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def SelectNodes(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/SelectNodes',
            NodeTools__pb2.SelectNodesRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def ReparentNodes(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/ReparentNodes',
            NodeTools__pb2.ReparentNodesRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def CopyNodeProperties(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/CopyNodeProperties',
            NodeTools__pb2.CopyNodePropertiesRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetNodeTypes(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetNodeTypes',
            NodeTools__pb2.Empty.SerializeToString,
            NodeTools__pb2.NodeTypesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetAllNodesByType(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetAllNodesByType',
            NodeTools__pb2.NodeTypeRequest.SerializeToString,
            NodeTools__pb2.NodesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetNodeProperties(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetNodeProperties',
            NodeTools__pb2.PathRequest.SerializeToString,
            NodeTools__pb2.PropertiesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def CreateAnimation(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/CreateAnimation',
            NodeTools__pb2.CreateAnimationRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def DeleteAnimation(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/DeleteAnimation',
            NodeTools__pb2.DeleteAnimationRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def SetAnimationSpeed(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/SetAnimationSpeed',
            NodeTools__pb2.SetAnimationSpeedRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def RenameAnimation(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/RenameAnimation',
            NodeTools__pb2.RenameAnimationRequest.SerializeToString,
            NodeTools__pb2.OperationResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)

    @staticmethod
    def GetAnimationNames(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(
            request,
            target,
            '/nodetools.NodeToolService/GetAnimationNames',
            NodeTools__pb2.PathRequest.SerializeToString,
            NodeTools__pb2.AnimationNamesResponse.FromString,
            options,
            channel_credentials,
            insecure,
            call_credentials,
            compression,
            wait_for_ready,
            timeout,
            metadata,
            _registered_method=True)
