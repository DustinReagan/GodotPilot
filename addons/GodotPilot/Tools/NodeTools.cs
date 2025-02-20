using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using Ollama;
using CSharpToJsonSchema;
using System.Buffers;

namespace GodotPilotPlugin.Tools;

[GenerateJsonSchema]
public interface INodeQueryTools
{
    public Task<string[]> GetAllNodes(CancellationToken cancellationToken = default);
    public Task<string[]> GetSelectedNodes(CancellationToken cancellationToken = default);
    public Task<string[]> GetNodesWithName(string name, CancellationToken cancellationToken = default);
    public string[] GetNodeTypes();
    public Task<string[]> GetAllNodesByType(string typeName, CancellationToken cancellationToken = default);
}

[GenerateJsonSchema]
public interface INodeToolFunctions
{
    public Task<string> CreateNode(string typeName, string parentPath, string name, CancellationToken cancellationToken = default);

    public Task<string> DeleteNode(string path, CancellationToken cancellationToken = default);

    public Task<string> RenameNode(string path, string newName, CancellationToken cancellationToken = default);

    public Task<string> ChangeNodeType(string path, string newType, CancellationToken cancellationToken = default);

    public Task<string> CopyNodeProperties(string sourcePath, string[] targetPaths, CancellationToken cancellationToken = default);

    public Task<string> DuplicateNode(string path, string newName, CancellationToken cancellationToken = default);

    public Task<string> SetNodeProperty(string path, string propertyName, string value, CancellationToken cancellationToken = default);

    public Task<Dictionary<string, string>> GetNodeProperties(string path, CancellationToken cancellationToken = default);

    public Task<string> SelectNodes(string[] paths, CancellationToken cancellationToken = default);

    public Task<string> ReparentNodes(string[] paths, string parentPath, CancellationToken cancellationToken = default);

    //public Task<string> CreateAnimation(string animatedSpritePath, string animationName, float? fps = null, CancellationToken cancellationToken = default);
    public string[] GetNodeTypes();
}

public interface IAnimationTools
{
    public Task<string> CreateAnimation(string animatedSpritePath, string animationName, float? fps = null, CancellationToken cancellationToken = default);
    public Task<string> DeleteAnimation(string animatedSpritePath, string animationName, CancellationToken cancellationToken = default);
    public Task<string> SetAnimationSpeed(string animatedSpritePath, string animationName, float fps, CancellationToken cancellationToken = default);
    public Task<string> RenameAnimation(string animatedSpritePath, string oldAnimationName, string newAnimationName, CancellationToken cancellationToken = default);

    public Task<List<string>> GetAnimationNames(string animatedSpritePath, CancellationToken cancellationToken = default);
}

public class NodeTools : INodeToolFunctions, IAnimationTools, INodeQueryTools
{
    [Description("Returns the types of nodes that can be created")]
    public string[] GetNodeTypes()
    {
        return NodeQueryToolsStatic.GetNodeTypes();
    }

    [Description("Returns the absolute paths of all currently selected nodes.")]
    public async Task<string[]> GetSelectedNodes(CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeQueryToolsStatic.GetSelectedNodes());
    }

    [Description("Returns the absolute paths of all nodes in the scene.")]
    public async Task<string[]> GetAllNodes(CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeQueryToolsStatic.GetAllNodes());
    }

    [Description("Creates a node of the given type, with the given name & parent, and returns the path of the new node.")]
    public async Task<string> CreateNode(
        [Description("The type of node to create (must exist in GetNodeTypes tool)")] string typeName,
        [Description("The absolute path of the desired parent node")] string parentPath,
        [Description("The desired name of the new node")] string name,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.CreateNode(typeName, parentPath, name));
    }

    [Description("Returns the absolute paths of all nodes with the given name.")]
    public async Task<string[]> GetNodesWithName(
        [Description("The name of the nodes to find")] string name,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeQueryToolsStatic.GetNodesWithName(name));
    }

    [Description("Deletes the node at the specified path and returns a status message.")]
    public async Task<string> DeleteNode(
        [Description("The absolute path of the node to delete")] string path,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.DeleteNode(path));
    }

    [Description("Renames the node at the specified path and returns a status message.")]
    public async Task<string> RenameNode(
        [Description("The absolute path of the node to rename")] string path,
        [Description("The new name for the node")] string newName,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.RenameNode(path, newName));
    }

    [Description("Changes the type of the node at the specified path and returns a status message.")]
    public async Task<string> ChangeNodeType(
        [Description("The absolute path of the node to change")] string path,
        [Description("The new type for the node (must exist in GetNodeTypes)")] string newType,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.ChangeNodeType(path, newType));
    }

    [Description("Duplicates/Copies the node at the specified path and returns the path of the new node.")]
    public async Task<string> DuplicateNode(
        [Description("The absolute path of the node to duplicate")] string path,
        [Description("The desired name of the new node")] string newName,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.DuplicateNode(path, newName));
    }

    [Description("Sets the property of the node at the specified path and returns a status message.")]
    public async Task<string> SetNodeProperty(
        [Description("The absolute path of the node to set the property on")] string path,
        [Description("The name of the property to set")] string propertyName,
        [Description("The value to set the property to as a string")] string value,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.SetNodeProperty(path, propertyName, value));
    }

    [Description("Returns the properties of the node at the specified path.")]
    public async Task<Dictionary<string, string>> GetNodeProperties(
        [Description("The absolute path of the node to get the properties of")] string path,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.GetNodeProperties(path));
    }

    [Description("Selects the nodes at the specified paths.")]
    public async Task<string> SelectNodes(
        [Description("The absolute paths of the nodes to select")] string[] paths,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.SelectNodes(paths));
    }

    [Description("Reparents the nodes at the specified paths to the new parent node and returns a status message.")]
    public async Task<string> ReparentNodes(
        [Description("The absolute paths of the nodes to reparent")] string[] paths,
        [Description("The absolute path of the parent node")] string parentPath,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.ReparentNodes(paths, parentPath));
    }

    [Description("Returns the absolute paths of all nodes of the given type.")]
    public async Task<string[]> GetAllNodesByType(
        [Description("The type of nodes to find")] string typeName,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeQueryToolsStatic.GetAllNodesByType(typeName));
    }

    [Description("Copies properties from a source node to one or more target nodes and returns a status message.")]
    public async Task<string> CopyNodeProperties(
        [Description("The absolute path of the source node")] string sourcePath,
        [Description("The absolute paths of the target nodes")] string[] targetPaths,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeToolsStatic.CopyNodeProperties(sourcePath, targetPaths));
    }

    [Description("Creates an animation for the given AnimatedSprite2D node and returns a status message.")]
    public async Task<string> CreateAnimation(
        [Description("The absolute path of the AnimatedSprite2D node")] string animatedSpritePath,
        [Description("The name of the animation to create")] string animationName,
        [Description("The speed of the animation in frames per second")] float? fps = null,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeAnimationToolsStatic.CreateAnimation(animatedSpritePath, animationName, fps));
    }

    [Description("Deletes the animation with the given name from the given AnimatedSprite2D node and returns a status message.")]
    public async Task<string> DeleteAnimation(
        [Description("The absolute path of the AnimatedSprite2D node")] string animatedSpritePath,
        [Description("The name of the animation to delete")] string animationName,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeAnimationToolsStatic.DeleteAnimation(animatedSpritePath, animationName));
    }

    [Description("Sets the speed (fps) of the animation with the given name from the given AnimatedSprite2D node and returns a status message.")]
    public async Task<string> SetAnimationSpeed(
        [Description("The absolute path of the AnimatedSprite2D node")] string animatedSpritePath,
        [Description("The name of the animation to set the speed of")] string animationName,
        [Description("The speed of the animation in frames per second")] float fps,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeAnimationToolsStatic.SetAnimationSpeed(animatedSpritePath, animationName, fps));
    }

    [Description("Renames the animation with the given name from the given AnimatedSprite2D node and returns a status message.")]
    public async Task<string> RenameAnimation(
        [Description("The absolute path of the AnimatedSprite2D node")] string animatedSpritePath,
        [Description("The name of the animation to rename")] string oldAnimationName,
        [Description("The new name for the animation")] string newAnimationName,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeAnimationToolsStatic.RenameAnimation(animatedSpritePath, oldAnimationName, newAnimationName));
    }

    [Description("Returns the names of all animations for the given AnimatedSprite2D node.")]
    public async Task<List<string>> GetAnimationNames(
        [Description("The absolute path of the AnimatedSprite2D node")] string animatedSpritePath,
        CancellationToken cancellationToken = default)
    {
        return await MainThreadHelper.RunOnMainThread(() => NodeAnimationToolsStatic.GetAnimationNames(animatedSpritePath));
    }
}

public class NodeQueryTools: INodeQueryTools
{
    public Task<string[]> GetAllNodes(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NodeQueryToolsStatic.GetAllNodes());
    }

    public Task<string[]> GetSelectedNodes(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NodeQueryToolsStatic.GetSelectedNodes());
    }

    public Task<string[]> GetNodesWithName(string name, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NodeQueryToolsStatic.GetNodesWithName(name));
    }

    public string[] GetNodeTypes()
    {
        return NodeQueryToolsStatic.GetNodeTypes();
    }

    public Task<string[]> GetAllNodesByType(string typeName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(NodeQueryToolsStatic.GetAllNodesByType(typeName));
    }
}