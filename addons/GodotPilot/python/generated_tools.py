#!/usr/bin/env python
"""
this file is dynamically generated; do not edit directly.
"""

import grpc
from langchain.tools import tool
from typing import Annotated, List
import NodeTools_pb2
import NodeTools_pb2_grpc

# create channel and stub
channel = grpc.insecure_channel('localhost:5001')
stub = NodeTools_pb2_grpc.NodeToolServiceStub(channel)

@tool
def GetAllNodes() -> List[str]:
    """Returns the absolute paths of all nodes in the scene"""
    request = NodeTools_pb2.Empty()
    response = stub.GetAllNodes(request)
    return response.nodes

@tool
def GetSelectedNodes() -> List[str]:
    """Returns the absolute paths of all currently selected nodes"""
    request = NodeTools_pb2.Empty()
    response = stub.GetSelectedNodes(request)
    return response.nodes

@tool
def GetNodesWithName(
    name: Annotated[str, "the name of the nodes to find"]
) -> List[str]:
    """Returns the absolute paths of all nodes with the given name"""
    request = NodeTools_pb2.NameRequest(name=name)
    response = stub.GetNodesWithName(request)
    return response.nodes

@tool
def CreateNode(
    typeName: Annotated[str, "the type of node to create (must exist in GetNodeTypes tool)"],
    parentPath: Annotated[str, "the absolute path of the desired parent node"],
    name: Annotated[str, "the desired name of the new node"]
) -> str:
    """Creates a node of the given type, with the given name & parent, and returns the path of the new node"""
    request = NodeTools_pb2.CreateNodeRequest(typeName=typeName, parentPath=parentPath, name=name)
    response = stub.CreateNode(request)
    return response.result

@tool
def DeleteNode(
    path: Annotated[str, "the absolute path of the node to affect"]
) -> str:
    """Deletes the node at the specified path and returns a status message"""
    request = NodeTools_pb2.PathRequest(path=path)
    response = stub.DeleteNode(request)
    return response.result

@tool
def RenameNode(
    path: Annotated[str, "the absolute path of the node to rename"],
    newName: Annotated[str, "the new name for the node"]
) -> str:
    """Renames the node at the specified path and returns a status message"""
    request = NodeTools_pb2.RenameNodeRequest(path=path, newName=newName)
    response = stub.RenameNode(request)
    return response.result

@tool
def ChangeNodeType(
    path: Annotated[str, "the absolute path of the node to change"],
    newType: Annotated[str, "the new type for the node (must exist in GetNodeTypes)"]
) -> str:
    """Changes the type of the node at the specified path and returns a status message"""
    request = NodeTools_pb2.ChangeNodeTypeRequest(path=path, newType=newType)
    response = stub.ChangeNodeType(request)
    return response.result

@tool
def DuplicateNode(
    path: Annotated[str, "the absolute path of the node to duplicate"],
    newName: Annotated[str, "the desired name of the new node"]
) -> str:
    """Duplicates/Copies the node at the specified path and returns the path of the new node"""
    request = NodeTools_pb2.DuplicateNodeRequest(path=path, newName=newName)
    response = stub.DuplicateNode(request)
    return response.result

@tool
def SetNodeProperty(
    path: Annotated[str, "the absolute path of the node to set the property on"],
    propertyName: Annotated[str, "the name of the property to set"],
    value: Annotated[str, "the value to set the property to as a string"]
) -> str:
    """Sets the property of the node at the specified path and returns a status message"""
    request = NodeTools_pb2.SetNodePropertyRequest(path=path, propertyName=propertyName, value=value)
    response = stub.SetNodeProperty(request)
    return response.result

@tool
def SelectNodes(
    paths: Annotated[List[str], "the absolute paths of the nodes to select"]
) -> str:
    """Selects the nodes at the specified paths"""
    request = NodeTools_pb2.SelectNodesRequest(paths=paths)
    response = stub.SelectNodes(request)
    return response.result

@tool
def ReparentNodes(
    paths: Annotated[List[str], "the absolute paths of the nodes to reparent"],
    parentPath: Annotated[str, "the absolute path of the parent node"]
) -> str:
    """Reparents the nodes at the specified paths to the new parent node and returns a status message"""
    request = NodeTools_pb2.ReparentNodesRequest(paths=paths, parentPath=parentPath)
    response = stub.ReparentNodes(request)
    return response.result

@tool
def CopyNodeProperties(
    sourcePath: Annotated[str, "the absolute path of the source node"],
    targetPaths: Annotated[List[str], "the absolute paths of the target nodes"]
) -> str:
    """Copies properties from a source node to one or more target nodes and returns a status message"""
    request = NodeTools_pb2.CopyNodePropertiesRequest(sourcePath=sourcePath, targetPaths=targetPaths)
    response = stub.CopyNodeProperties(request)
    return response.result

@tool
def GetNodeTypes() -> List[str]:
    """Returns the types of nodes that can be created"""
    request = NodeTools_pb2.Empty()
    response = stub.GetNodeTypes(request)
    return response.types

@tool
def GetAllNodesByType(
    typeName: Annotated[str, "the type of nodes to find"]
) -> List[str]:
    """Returns the absolute paths of all nodes of the given type"""
    request = NodeTools_pb2.NodeTypeRequest(typeName=typeName)
    response = stub.GetAllNodesByType(request)
    return response.nodes

@tool
def GetNodeProperties(
    path: Annotated[str, "the absolute path of the node to affect"]
) -> dict[str, str]:
    """Returns the properties of the node at the specified path"""
    request = NodeTools_pb2.PathRequest(path=path)
    response = stub.GetNodeProperties(request)
    return response.properties

@tool
def CreateAnimation(
    animatedSpritePath: Annotated[str, "the absolute path of the AnimatedSprite2D node"],
    animationName: Annotated[str, "the name of the animation to create"],
    fps: Annotated[float, "the speed of the animation in frames per second"]
) -> str:
    """Creates an animation for the given AnimatedSprite2D node and returns a status message"""
    request = NodeTools_pb2.CreateAnimationRequest(animatedSpritePath=animatedSpritePath, animationName=animationName, fps=fps)
    response = stub.CreateAnimation(request)
    return response.result

@tool
def DeleteAnimation(
    animatedSpritePath: Annotated[str, "the absolute path of the AnimatedSprite2D node"],
    animationName: Annotated[str, "the name of the animation to delete"]
) -> str:
    """Deletes the animation with the given name from the given AnimatedSprite2D node and returns a status message"""
    request = NodeTools_pb2.DeleteAnimationRequest(animatedSpritePath=animatedSpritePath, animationName=animationName)
    response = stub.DeleteAnimation(request)
    return response.result

@tool
def SetAnimationSpeed(
    animatedSpritePath: Annotated[str, "the absolute path of the AnimatedSprite2D node"],
    animationName: Annotated[str, "the name of the animation to set the speed of"],
    fps: Annotated[float, "the speed of the animation in frames per second"]
) -> str:
    """Sets the speed (fps) of the animation with the given name from the given AnimatedSprite2D node and returns a status message"""
    request = NodeTools_pb2.SetAnimationSpeedRequest(animatedSpritePath=animatedSpritePath, animationName=animationName, fps=fps)
    response = stub.SetAnimationSpeed(request)
    return response.result

@tool
def RenameAnimation(
    animatedSpritePath: Annotated[str, "the absolute path of the AnimatedSprite2D node"],
    oldAnimationName: Annotated[str, "the name of the animation to rename"],
    newAnimationName: Annotated[str, "the new name for the animation"]
) -> str:
    """Renames the animation with the given name from the given AnimatedSprite2D node and returns a status message"""
    request = NodeTools_pb2.RenameAnimationRequest(animatedSpritePath=animatedSpritePath, oldAnimationName=oldAnimationName, newAnimationName=newAnimationName)
    response = stub.RenameAnimation(request)
    return response.result

@tool
def GetAnimationNames(
    path: Annotated[str, "the absolute path of the node to affect"]
) -> List[str]:
    """Returns the names of all animations for the given AnimatedSprite2D node"""
    request = NodeTools_pb2.PathRequest(path=path)
    response = stub.GetAnimationNames(request)
    return response.names

tools = [
    GetAllNodes,
    GetSelectedNodes,
    GetNodesWithName,
    CreateNode,
    DeleteNode,
    RenameNode,
    ChangeNodeType,
    DuplicateNode,
    SetNodeProperty,
    SelectNodes,
    ReparentNodes,
    CopyNodeProperties,
    GetNodeTypes,
    GetAllNodesByType,
    GetNodeProperties,
    CreateAnimation,
    DeleteAnimation,
    SetAnimationSpeed,
    RenameAnimation,
    GetAnimationNames,
]
