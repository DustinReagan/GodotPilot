using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
namespace GodotPilotPlugin.Tools;

public static class NodeQueryToolsStatic
{
    private static Dictionary<string, Type> _nodeTypes = InitializeNodeTypes();

    private static Dictionary<string, Type> InitializeNodeTypes()
    {
        var nodeTypes = new Dictionary<string, Type>();
        var nodeBaseType = typeof(Node);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && nodeBaseType.IsAssignableFrom(type));

                foreach (var type in types)
                {
                    nodeTypes[type.Name] = type;
                }
            }
            catch (ReflectionTypeLoadException)
            {
                continue;
            }
        }

        return nodeTypes;
    }

    public static string[] GetNodeTypes()
    {
        return _nodeTypes.Keys.ToArray();
    }

    public static Dictionary<string, Type> GetNodeTypeDictionary()
    {
        return _nodeTypes;
    }

    public static string[] GetAllNodes()
    {
        List<Node> GetChildren(Node node)
        {
            var nodes = new List<Node>{node};
            foreach (var child in node.GetChildren())
            {
                nodes.AddRange(GetChildren(child));
            }
            return nodes;
        }
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var allNodes = GetChildren(root);
        allNodes.RemoveAt(0);
        var paths = allNodes.Select(node => $"./{root.GetPathTo(node)}").ToList();

        paths.Insert(0, ".");
        return paths.ToArray();
    }

    public static string[] GetSelectedNodes()
    {
        var selection = EditorInterface.Singleton.GetSelection();
        var selectedNodes = selection?.GetSelectedNodes();
        if (selectedNodes == null || selectedNodes.Count == 0)
        {
            return Array.Empty<string>();
        }
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        return selectedNodes.Select(node => $"./{root.GetPathTo(node)}").ToArray();
    }

    public static string[] GetNodesWithName(string name)
    {
        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var allNodes = GetAllNodes();
        var nodesWithName = allNodes.Where(path =>
            root.GetNode(path).Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase)
        ).Select(path => $"./{root.GetPathTo(root.GetNode(path))}");
        return nodesWithName.Any() ? nodesWithName.ToArray() : new string[]{"Error: No nodes with name: " + name +" found.  Try calling GetAllNodes"};
    }

    public static string[] GetAllNodesByType(string typeName)
    {
        if (!_nodeTypes.ContainsKey(typeName))
        {
            return new string[]{"Error: unknown node type: " + typeName + ". Call GetNodeTypes to get a list of valid node types."};
        }

        var root = EditorInterface.Singleton.GetEditedSceneRoot();
        var nodes = root.GetChildren().Where(node => node.GetType().Name == typeName).Select(node => $"./{root.GetPathTo(node)}").ToArray();
        return nodes;
    }
}