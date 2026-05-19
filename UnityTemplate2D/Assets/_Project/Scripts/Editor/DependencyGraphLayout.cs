using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DependencyGraphLayout
{
    const float NodeHeight = 60f;
    const float HSpacing = 50f;
    const float VSpacing = 90f;

    public static void ApplyLayout(DependencyGraph graph, float startX, float startY)
    {
        var layers = ComputeLayers(graph);
        var groups = GroupByLayer(graph, layers);
        OrderByBarycenter(groups, graph, layers);
        AssignPositions(groups, startX, startY);
    }

    static Dictionary<string, int> ComputeLayers(DependencyGraph graph)
    {
        var layers = new Dictionary<string, int>();
        var visited = new HashSet<string>();
        var inStack = new HashSet<string>();

        foreach (var node in graph.Nodes)
            GetLayer(node.TypeName, graph, layers, visited, inStack);

        int maxLayer = layers.Count > 0 ? layers.Values.Max() : 0;
        foreach (var key in layers.Keys.ToList())
            layers[key] = maxLayer - layers[key];

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

    static Dictionary<int, List<ScriptNode>> GroupByLayer(DependencyGraph graph, Dictionary<string, int> layers)
    {
        var groups = new Dictionary<int, List<ScriptNode>>();
        foreach (var node in graph.Nodes)
        {
            int layer = layers.TryGetValue(node.TypeName, out var l) ? l : 0;
            if (!groups.ContainsKey(layer))
                groups[layer] = new List<ScriptNode>();
            groups[layer].Add(node);
        }
        return groups;
    }

    static void OrderByBarycenter(Dictionary<int, List<ScriptNode>> groups, DependencyGraph graph,
        Dictionary<string, int> layers)
    {
        int maxLayer = groups.Keys.Count > 0 ? groups.Keys.Max() : 0;

        if (groups.ContainsKey(0))
            groups[0].Sort((a, b) => string.Compare(a.TypeName, b.TypeName));

        for (int pass = 0; pass < 3; pass++)
        {
            for (int layer = 1; layer <= maxLayer; layer++)
            {
                if (!groups.ContainsKey(layer)) continue;
                var prevLayer = groups.ContainsKey(layer - 1) ? groups[layer - 1] : null;
                if (prevLayer == null) continue;

                var prevPositions = new Dictionary<string, int>();
                for (int i = 0; i < prevLayer.Count; i++)
                    prevPositions[prevLayer[i].TypeName] = i;

                groups[layer].Sort((a, b) =>
                {
                    float avgA = GetBarycenter(a, prevPositions, graph, layers, layer);
                    float avgB = GetBarycenter(b, prevPositions, graph, layers, layer);
                    return avgA.CompareTo(avgB);
                });
            }
        }
    }

    static float GetBarycenter(ScriptNode node, Dictionary<string, int> prevPositions,
        DependencyGraph graph, Dictionary<string, int> layers, int currentLayer)
    {
        int sum = 0;
        int count = 0;

        foreach (var dep in node.DependsOn)
        {
            if (prevPositions.TryGetValue(dep, out int pos))
            {
                sum += pos;
                count++;
            }
        }

        foreach (var other in graph.Nodes)
        {
            if (other.DependsOn.Contains(node.TypeName))
            {
                int otherLayer = layers.TryGetValue(other.TypeName, out var l) ? l : 0;
                if (otherLayer == currentLayer - 1 && prevPositions.TryGetValue(other.TypeName, out int pos))
                {
                    sum += pos;
                    count++;
                }
            }
        }

        return count > 0 ? (float)sum / count : float.MaxValue;
    }

    static void AssignPositions(Dictionary<int, List<ScriptNode>> groups, float startX, float startY)
    {
        float widestLayer = 0f;
        foreach (var kvp in groups)
        {
            float w = ComputeLayerWidth(kvp.Value);
            if (w > widestLayer) widestLayer = w;
        }

        float centerX = startX + widestLayer / 2f;

        foreach (var kvp in groups)
        {
            var nodesInLayer = kvp.Value;
            float layerWidth = ComputeLayerWidth(nodesInLayer);
            float layerStartX = centerX - layerWidth / 2f;
            float y = startY + kvp.Key * (NodeHeight + VSpacing);

            float x = layerStartX;
            for (int i = 0; i < nodesInLayer.Count; i++)
            {
                float nodeWidth = ComputeNodeWidth(nodesInLayer[i].TypeName);
                nodesInLayer[i].Rect = new Rect(x, y, nodeWidth, NodeHeight);
                x += nodeWidth + HSpacing;
            }
        }
    }

    static float ComputeLayerWidth(List<ScriptNode> nodes)
    {
        float total = 0f;
        for (int i = 0; i < nodes.Count; i++)
        {
            total += ComputeNodeWidth(nodes[i].TypeName);
            if (i < nodes.Count - 1) total += HSpacing;
        }
        return total;
    }

    public static float ComputeNodeWidth(string typeName)
    {
        float minWidth = 150f;
        float charWidth = 8f;
        float nameWidth = typeName.Length * charWidth + 40f;
        return Mathf.Max(minWidth, nameWidth);
    }
}
