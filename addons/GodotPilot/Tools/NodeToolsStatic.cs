using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GodotPilotPlugin.Tools;

public static class NodeToolsStatic
{
    public static string CreateNode(string typeName, string parentPath, string name)
    {
        var sceneRoot = EditorInterface.Singleton.GetEditedSceneRoot();
        var parent = sceneRoot.GetNode(parentPath);
        if (parent == null)
        {
            var error = $"Error: could not find parent node at path: {parentPath}. (Call GetAllNodes to get a list of all nodes in the scene)";
            GD.PrintErr(error);
            return error;
        }

        var nodeTypes = NodeQueryToolsStatic.GetNodeTypeDictionary();
        if (!nodeTypes.ContainsKey(typeName))
        {
            var error = $"Error: unknown node type: {typeName}. Call GetNodeTypes to get a list of valid node types.";
            GD.PrintErr(error);
            return error;
        }

        var node = (Node)Activator.CreateInstance(nodeTypes[typeName]);
        if(node == null)
        {
            var error = $"Error: could not create node of type: {typeName}.";
            GD.PrintErr(error);
            return error;
        }
        parent.AddChild(node);
        node.Owner = sceneRoot;
        node.Name = name;

        return $"Successfully created node at path: ./{sceneRoot.GetPathTo(node)}";
    }

    public static string DeleteNode(string path)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var node = root.GetNode(path);
        if (node == null)
        {
            var error = $"Error: could not find node at path: {path} (Call GetAllNodes to get a list of all nodes in the scene)";
            GD.PrintErr(error);
            return error;
        }

        node.QueueFree();
        return $"Successfully deleted node at path: {path}";
    }

    public static string RenameNode(string path, string newName)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var node = root.GetNode(path);
        if (node == null)
        {
            var error = $"Error: could not find node at path: {path} (Call GetAllNodes to get a list of all nodes in the scene)";
            GD.PrintErr(error);
            return error;
        }

        node.Name = newName;
        return $"Successfully renamed node to: {newName}";
    }

    public static string ChangeNodeType(string path, string newType)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var oldNode = root.GetNode(path);
        if (oldNode == null)
        {
            var error = $"Error: could not find node at path: {path} (Call GetAllNodes to get a list of all nodes in the scene)";
            GD.PrintErr(error);
            return error;
        }

        var nodeTypes = NodeQueryToolsStatic.GetNodeTypeDictionary();
        if (!nodeTypes.ContainsKey(newType))
        {
            var error = $"Error: unknown node type: {newType}. Call GetNodeTypes to get a list of valid node types.";
            GD.PrintErr(error);
            return error;
        }

        var newNode = (Node)Activator.CreateInstance(nodeTypes[newType]);
        if (newNode == null)
        {
            var error = $"Error: could not create node of type: {newType}";
            GD.PrintErr(error);
            return error;
        }

        newNode.Name = oldNode.Name;
        newNode.Owner = root;

        oldNode.ReplaceBy(newNode, true);
        oldNode.QueueFree();

        return $"Successfully changed node with path: {path} to type: {newType}";
    }

    public static string DuplicateNode(string path, string newName)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var node = root.GetNode(path);
        if (node == null)
        {
            var error = $"Error: could not find node at path: {path} (Call GetAllNodes to get a list of all nodes in the scene)";
            GD.PrintErr(error);
            return error;
        }

        var newNode = node.Duplicate();
        newNode.Name = newName;
        newNode.Owner = root;
        node.GetParent().AddChild(newNode);

        SetOwnerRecursive(newNode, node.Owner);

        return $"./{root.GetPathTo(newNode)}";
    }

    private static void SetOwnerRecursive(Node current, Node owner)
    {
        current.Owner = owner;
        foreach (Node child in current.GetChildren())
        {
            SetOwnerRecursive(child, owner);
        }
    }

    public static string SetNodeProperty(string path, string propertyName, string value)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var node = root.GetNode(path);
        if (node == null)
        {
            return $"Error: could not find node at path: {path} (Call GetAllNodes to get a list of all nodes in the scene)";
        }

        var variant = Variant.From(value);
        node.Set(propertyName, variant);
        return $"Successfully set property {propertyName} to {value} on node at path: {path}";
    }

    public static Dictionary<string, string> GetNodeProperties(string path)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var node = root.GetNode(path);
        return node.GetPropertyList().Select(prop => (string)prop["name"]).ToDictionary(name => name, name => node.Get(name).AsString());
    }

    public static string SelectNodes(string[] paths)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        if (root == null)
        {
            return "Error: no scene is currently being edited";
        }

        var selection = EditorInterface.Singleton.GetSelection();
        if (selection == null)
        {
            return "Error: could not access editor selection";
        }

        selection.Clear();

        var selectedCount = 0;
        var errors = new List<string>();

        foreach (var path in paths)
        {
            try
            {
                var node = root.GetNode(path);
                if (node == null)
                {
                    errors.Add($"could not find node at path: {path} (Call GetAllNodes to get a list of all nodes in the scene)");
                    continue;
                }

                selection.AddNode(node);
                selectedCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"error selecting node at path {path}: {ex.Message}");
            }
        }

        return errors.Count == 0
            ? $"Successfully selected {selectedCount} node(s)"
            : $"Selected {selectedCount} node(s) with {errors.Count} error(s):\n - {string.Join("\n - ", errors)}";
    }

    public static string ReparentNodes(string[] paths, string parentPath)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        if (root == null)
        {
            return "Error: no scene is currently being edited";
        }

        var successCount = 0;
        var errors = new List<string>();

        foreach (var path in paths)
        {
            var result = ReparentNode(path, parentPath);
            if (result.StartsWith("Successfully"))
            {
                successCount++;
            }
            else
            {
                errors.Add(result.Replace("Error: ", ""));
            }
        }

        return errors.Count == 0
            ? $"Successfully reparented {successCount} node(s)"
            : $"Reparented {successCount} node(s) with {errors.Count} error(s):\n - {string.Join("\n - ", errors)}";
    }

    private static string ReparentNode(string path, string parentPath)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        if (root == null)
        {
            return "Error: no scene is currently being edited";
        }

        var node = root.GetNode(path);
        if (node == null)
        {
            return $"Error: could not find node at path: {path}";
        }

        var newParent = root.GetNode(parentPath);
        if (newParent == null)
        {
            return $"Error: could not find parent node at path: {parentPath} (Call GetAllNodes to get a list of all nodes in the scene)";
        }

        if (node == newParent || node.IsAncestorOf(newParent))
        {
            return $"Error: cannot reparent node at path {path} to itself or to its child {parentPath} {newParent.Name}";
        }

        try
        {
            var oldParent = node.GetParent();
            oldParent.RemoveChild(node);
            newParent.AddChild(node);
            node.Owner = root;
            return $"Successfully reparented node at path: {path}";
        }
        catch (Exception ex)
        {
            return $"Error reparenting node at path {path}: {ex.Message}";
        }
    }

    public static string CopyNodeProperties(string sourcePath, string[] targetPaths)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();

        var sourceNode = root.GetNode(sourcePath);
        if (sourceNode == null)
        {
            var error = $"Error: could not find source node at path: {sourcePath} (Call GetAllNodes to get a list of all nodes in the scene)";
            GD.PrintErr(error);
            return error;
        }

        var successCount = 0;
        var errorCount = 0;

        foreach (var targetPath in targetPaths)
        {
            var targetNode = root.GetNode(targetPath);
            if (targetNode == null)
            {
                GD.PrintErr($"Error: could not find target node at path: {targetPath} (Call GetAllNodes to get a list of all nodes in the scene)");
                errorCount++;
                continue;
            }

            if (sourceNode == targetNode)
            {
                GD.PrintErr($"Warning: skipping self-copy for node at path: {targetPath}");
                continue;
            }

            if (sourceNode.GetType() != targetNode.GetType())
            {
                GD.PrintErr($"Error: type mismatch between source ({sourceNode.GetType().Name}) and target ({targetNode.GetType().Name}) for path: {targetPath}");
                errorCount++;
                continue;
            }

            try
            {
                var properties = sourceNode.GetPropertyList()
                    .Where(prop => ((PropertyUsageFlags)(long)prop["usage"]).HasFlag(PropertyUsageFlags.Storage))
                    .Select(prop => (string)prop["name"]);

                foreach (var propertyName in properties)
                {
                    var value = sourceNode.Get(propertyName);
                    targetNode.Set(propertyName, value);
                }

                successCount++;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error copying properties to node at path {targetPath}: {ex.Message}");
                errorCount++;
            }
        }

        return $"Property copy complete. Success: {successCount}, Errors: {errorCount}";
    }
}