using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DependencyGraphLayout
{
    const float NodeHeight = 60f;
    const float DummyWidth = 8f;
    const float HSpacing = 50f;
    const float VSpacing = 90f;

    public static void ApplyLayout(DependencyGraph graph, float startX, float startY)
    {
        if (graph.Nodes.Count == 0) return;

        var layers = ComputeLayers(graph);
        var layerGroups = GroupByLayer(graph, layers);
        int maxLayer = layerGroups.Keys.Count > 0 ? layerGroups.Keys.Max() : 0;

        var virtualNodes = new Dictionary<int, List<string>>();
        var virtualEdges = new List<VirtualEdge>();
        InsertDummyNodes(graph, layers, layerGroups, maxLayer, virtualNodes, virtualEdges);

        MinimizeCrossings(layerGroups, virtualNodes, virtualEdges, graph, layers, maxLayer);

        AssignPositions(layerGroups, virtualNodes, startX, startY);
    }

    #region Layer Assignment

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

    #endregion

    #region Dummy Nodes

    struct VirtualEdge
    {
        public string From;
        public int FromLayer;
        public string To;
        public int ToLayer;
    }

    static void InsertDummyNodes(DependencyGraph graph, Dictionary<string, int> layers,
        Dictionary<int, List<ScriptNode>> layerGroups, int maxLayer,
        Dictionary<int, List<string>> virtualNodes, List<VirtualEdge> virtualEdges)
    {
        int dummyId = 0;

        foreach (var node in graph.Nodes)
        {
            int srcLayer = layers[node.TypeName];

            foreach (var dep in node.DependsOn)
            {
                if (!layers.TryGetValue(dep, out int dstLayer)) continue;

                int topLayer = Mathf.Min(srcLayer, dstLayer);
                int botLayer = Mathf.Max(srcLayer, dstLayer);
                int span = botLayer - topLayer;

                if (span <= 1)
                {
                    virtualEdges.Add(new VirtualEdge
                    {
                        From = node.TypeName, FromLayer = srcLayer,
                        To = dep, ToLayer = dstLayer
                    });
                    continue;
                }

                string prev = node.TypeName;
                int prevLayer = srcLayer;
                int dir = srcLayer < dstLayer ? 1 : -1;

                for (int l = srcLayer + dir; l != dstLayer; l += dir)
                {
                    string dummyName = $"__dummy_{dummyId++}";

                    if (!virtualNodes.ContainsKey(l))
                        virtualNodes[l] = new List<string>();
                    virtualNodes[l].Add(dummyName);

                    virtualEdges.Add(new VirtualEdge
                    {
                        From = prev, FromLayer = prevLayer,
                        To = dummyName, ToLayer = l
                    });

                    prev = dummyName;
                    prevLayer = l;
                }

                virtualEdges.Add(new VirtualEdge
                {
                    From = prev, FromLayer = prevLayer,
                    To = dep, ToLayer = dstLayer
                });
            }
        }

        foreach (var kvp in virtualNodes)
        {
            if (!layerGroups.ContainsKey(kvp.Key))
                layerGroups[kvp.Key] = new List<ScriptNode>();
        }
    }

    #endregion

    #region Crossing Minimization

    static void MinimizeCrossings(Dictionary<int, List<ScriptNode>> layerGroups,
        Dictionary<int, List<string>> virtualNodes, List<VirtualEdge> virtualEdges,
        DependencyGraph graph, Dictionary<string, int> layers, int maxLayer)
    {
        var layerOrder = new Dictionary<int, List<string>>();
        for (int l = 0; l <= maxLayer; l++)
        {
            var order = new List<string>();
            if (layerGroups.ContainsKey(l))
            {
                foreach (var n in layerGroups[l])
                    order.Add(n.TypeName);
            }
            if (virtualNodes.ContainsKey(l))
                order.AddRange(virtualNodes[l]);

            order.Sort();
            layerOrder[l] = order;
        }

        var adjUp = BuildAdjacency(virtualEdges, maxLayer, up: true);
        var adjDown = BuildAdjacency(virtualEdges, maxLayer, up: false);

        for (int pass = 0; pass < 12; pass++)
        {
            if (pass % 2 == 0)
            {
                for (int l = 1; l <= maxLayer; l++)
                    SortLayerByBarycenter(layerOrder, l, layerOrder[l - 1], adjUp);
            }
            else
            {
                for (int l = maxLayer - 1; l >= 0; l--)
                    SortLayerByBarycenter(layerOrder, l, layerOrder[l + 1], adjDown);
            }
        }

        for (int l = 0; l <= maxLayer; l++)
        {
            if (!layerGroups.ContainsKey(l)) continue;

            var posMap = new Dictionary<string, int>();
            for (int i = 0; i < layerOrder[l].Count; i++)
                posMap[layerOrder[l][i]] = i;

            layerGroups[l].Sort((a, b) =>
            {
                int posA = posMap.TryGetValue(a.TypeName, out var pa) ? pa : 999;
                int posB = posMap.TryGetValue(b.TypeName, out var pb) ? pb : 999;
                return posA.CompareTo(posB);
            });

            if (virtualNodes.ContainsKey(l))
            {
                virtualNodes[l].Sort((a, b) =>
                {
                    int posA = posMap.TryGetValue(a, out var pa) ? pa : 999;
                    int posB = posMap.TryGetValue(b, out var pb) ? pb : 999;
                    return posA.CompareTo(posB);
                });
            }
        }
    }

    static Dictionary<string, List<string>> BuildAdjacency(List<VirtualEdge> edges, int maxLayer, bool up)
    {
        var adj = new Dictionary<string, List<string>>();
        foreach (var e in edges)
        {
            string child = up ? (e.FromLayer > e.ToLayer ? e.From : e.To) : (e.FromLayer < e.ToLayer ? e.From : e.To);
            string parent = up ? (e.FromLayer > e.ToLayer ? e.To : e.From) : (e.FromLayer < e.ToLayer ? e.To : e.From);

            if (!adj.ContainsKey(child))
                adj[child] = new List<string>();
            adj[child].Add(parent);
        }
        return adj;
    }

    static void SortLayerByBarycenter(Dictionary<int, List<string>> layerOrder, int layer,
        List<string> refLayer, Dictionary<string, List<string>> adj)
    {
        var refPos = new Dictionary<string, int>();
        for (int i = 0; i < refLayer.Count; i++)
            refPos[refLayer[i]] = i;

        var items = layerOrder[layer];
        items.Sort((a, b) =>
        {
            float bcA = GetBarycenterValue(a, refPos, adj);
            float bcB = GetBarycenterValue(b, refPos, adj);
            return bcA.CompareTo(bcB);
        });
    }

    static float GetBarycenterValue(string nodeName, Dictionary<string, int> refPos,
        Dictionary<string, List<string>> adj)
    {
        if (!adj.TryGetValue(nodeName, out var neighbors) || neighbors.Count == 0)
            return float.MaxValue;

        float sum = 0;
        int count = 0;
        foreach (var n in neighbors)
        {
            if (refPos.TryGetValue(n, out int pos))
            {
                sum += pos;
                count++;
            }
        }
        return count > 0 ? sum / count : float.MaxValue;
    }

    #endregion

    #region Position Assignment

    static void AssignPositions(Dictionary<int, List<ScriptNode>> layerGroups,
        Dictionary<int, List<string>> virtualNodes, float startX, float startY)
    {
        int maxLayer = layerGroups.Keys.Count > 0 ? layerGroups.Keys.Max() : 0;

        float widestLayer = 0f;
        for (int l = 0; l <= maxLayer; l++)
        {
            float w = ComputeMixedLayerWidth(
                layerGroups.ContainsKey(l) ? layerGroups[l] : null,
                virtualNodes.ContainsKey(l) ? virtualNodes[l] : null);
            if (w > widestLayer) widestLayer = w;
        }

        float centerX = startX + widestLayer / 2f;

        for (int l = 0; l <= maxLayer; l++)
        {
            var realNodes = layerGroups.ContainsKey(l) ? layerGroups[l] : null;
            var dummies = virtualNodes.ContainsKey(l) ? virtualNodes[l] : null;

            float layerWidth = ComputeMixedLayerWidth(realNodes, dummies);
            float x = centerX - layerWidth / 2f;
            float y = startY + l * (NodeHeight + VSpacing);

            int realIdx = 0;
            int dummyIdx = 0;
            int totalReal = realNodes?.Count ?? 0;
            int totalDummy = dummies?.Count ?? 0;
            int total = totalReal + totalDummy;

            var realSet = new HashSet<string>();
            if (realNodes != null)
                foreach (var n in realNodes) realSet.Add(n.TypeName);

            var merged = new List<string>();
            if (realNodes != null) foreach (var n in realNodes) merged.Add(n.TypeName);
            if (dummies != null) merged.AddRange(dummies);

            foreach (var name in merged)
            {
                if (realSet.Contains(name))
                {
                    var node = realNodes.First(n => n.TypeName == name);
                    float nw = ComputeNodeWidth(name);
                    node.Rect = new Rect(x, y, nw, NodeHeight);
                    x += nw + HSpacing;
                }
                else
                {
                    x += DummyWidth + HSpacing;
                }
            }
        }
    }

    static float ComputeMixedLayerWidth(List<ScriptNode> realNodes, List<string> dummies)
    {
        float total = 0f;
        int count = 0;

        if (realNodes != null)
        {
            foreach (var n in realNodes)
            {
                if (count > 0) total += HSpacing;
                total += ComputeNodeWidth(n.TypeName);
                count++;
            }
        }

        if (dummies != null)
        {
            foreach (var d in dummies)
            {
                if (count > 0) total += HSpacing;
                total += DummyWidth;
                count++;
            }
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

    #endregion
}
