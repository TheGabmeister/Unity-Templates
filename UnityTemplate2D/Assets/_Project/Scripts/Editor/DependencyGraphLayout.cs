using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DependencyGraphLayout
{
    const float NodeWidth = 180f;
    const float NodeHeight = 60f;
    const float HSpacing = 60f;
    const float VSpacing = 80f;

    public static void ApplyLayout(DependencyGraph graph, float startX, float startY)
    {
        var layers = ComputeLayers(graph);
        AssignPositions(graph, layers, startX, startY);
    }

    static Dictionary<string, int> ComputeLayers(DependencyGraph graph)
    {
        var layers = new Dictionary<string, int>();
        var visited = new HashSet<string>();
        var inStack = new HashSet<string>();

        foreach (var node in graph.Nodes)
            GetLayer(node.TypeName, graph, layers, visited, inStack);

        return layers;
    }

    static int GetLayer(string typeName, DependencyGraph graph, Dictionary<string, int> layers,
        HashSet<string> visited, HashSet<string> inStack)
    {
        if (visited.Contains(typeName))
            return layers.TryGetValue(typeName, out var l) ? l : 0;

        if (inStack.Contains(typeName))
            return 0;

        if (!graph.NodesByType.TryGetValue(typeName, out var node))
            return 0;

        inStack.Add(typeName);

        int maxDepLayer = -1;
        foreach (var dep in node.DependsOn)
        {
            if (graph.NodesByType.ContainsKey(dep))
            {
                int depLayer = GetLayer(dep, graph, layers, visited, inStack);
                if (depLayer > maxDepLayer)
                    maxDepLayer = depLayer;
            }
        }

        inStack.Remove(typeName);

        int layer = maxDepLayer + 1;
        layers[typeName] = layer;
        visited.Add(typeName);
        return layer;
    }

    static void AssignPositions(DependencyGraph graph, Dictionary<string, int> layers, float startX, float startY)
    {
        int maxLayer = layers.Count > 0 ? layers.Values.Max() : 0;

        var groups = new Dictionary<int, List<ScriptNode>>();
        foreach (var node in graph.Nodes)
        {
            int layer = layers.TryGetValue(node.TypeName, out var l) ? l : 0;
            if (!groups.ContainsKey(layer))
                groups[layer] = new List<ScriptNode>();
            groups[layer].Add(node);
        }

        foreach (var kvp in groups)
        {
            var nodesInLayer = kvp.Value;
            nodesInLayer.Sort((a, b) => string.Compare(a.TypeName, b.TypeName));

            float y = startY + kvp.Key * (NodeHeight + VSpacing);
            for (int i = 0; i < nodesInLayer.Count; i++)
            {
                float nodeWidth = ComputeNodeWidth(nodesInLayer[i].TypeName);
                float x = startX + i * (NodeWidth + HSpacing);
                nodesInLayer[i].Rect = new Rect(x, y, nodeWidth, NodeHeight);
            }
        }
    }

    public static float ComputeNodeWidth(string typeName)
    {
        float minWidth = 150f;
        float charWidth = 8f;
        float nameWidth = typeName.Length * charWidth + 40f;
        return Mathf.Max(minWidth, nameWidth);
    }
}
