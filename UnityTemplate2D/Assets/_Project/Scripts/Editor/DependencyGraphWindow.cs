using UnityEditor;
using UnityEngine;

public class DependencyGraphWindow : EditorWindow
{
    DependencyGraph _graph;
    Vector2 _panOffset;
    bool _isPanning;
    Vector2 _panStartMouse;
    Vector2 _panStartOffset;
    bool _scanRequested;

    const float ToolbarHeight = 21f;
    const float ArrowSize = 12f;
    const float ArrowAngle = 20f;
    const float EdgeWidth = 2.5f;

    static GUIStyle _nodeStyle;
    static GUIStyle _titleStyle;
    static GUIStyle _subtitleStyle;

    [MenuItem("Tools/Dependency Graph")]
    static void Open()
    {
        var window = GetWindow<DependencyGraphWindow>("Dependency Graph");
        window.minSize = new Vector2(600, 400);
    }

    void OnGUI()
    {
        if (_scanRequested && Event.current.type == EventType.Layout)
        {
            _scanRequested = false;
            _graph = DependencyScanner.Scan();
            if (_graph != null && _graph.Nodes.Count > 0)
            {
                DependencyGraphLayout.ApplyLayout(_graph, 50f, 50f);
                CenterGraph();
            }
            Repaint();
        }

        InitStyles();
        DrawToolbar();

        if (_graph == null || _graph.Nodes.Count == 0)
        {
            DrawEmptyState();
            return;
        }

        HandlePanning();
        DrawEdges();
        DrawNodes();

        if (Event.current.type == EventType.MouseDrag)
            Repaint();
    }

    void InitStyles()
    {
        if (_nodeStyle != null) return;

        _nodeStyle = new GUIStyle("flow node 0")
        {
            padding = new RectOffset(10, 10, 8, 8)
        };
        _titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = 11,
            normal = { textColor = Color.white }
        };
        _subtitleStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.UpperCenter,
            normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
        };
    }

    void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Scan", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            _scanRequested = true;
            Repaint();
        }

        if (_graph != null && _graph.Nodes.Count > 0)
        {
            int edgeCount = 0;
            foreach (var n in _graph.Nodes)
                edgeCount += n.DependsOn.Count;
            GUILayout.Label($"{_graph.Nodes.Count} scripts, {edgeCount} dependencies");
        }
        else
        {
            GUILayout.Label("Click Scan to analyze scripts");
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void DrawEmptyState()
    {
        GUILayout.Space(40);
        EditorGUILayout.HelpBox(
            "No scripts found under Assets/_Project/Scripts/ (excluding Editor/).\n" +
            "Add scripts and click Scan.",
            MessageType.Info);
    }

    void DrawNodes()
    {
        BeginWindows();
        for (int i = 0; i < _graph.Nodes.Count; i++)
        {
            var node = _graph.Nodes[i];
            var screenRect = new Rect(
                node.Rect.x + _panOffset.x,
                node.Rect.y + _panOffset.y + ToolbarHeight,
                node.Rect.width,
                node.Rect.height);

            var newRect = GUI.Window(i, screenRect, DrawNodeContent, GUIContent.none, _nodeStyle);

            node.Rect.x = newRect.x - _panOffset.x;
            node.Rect.y = newRect.y - _panOffset.y - ToolbarHeight;
        }
        EndWindows();
    }

    void DrawNodeContent(int id)
    {
        var node = _graph.Nodes[id];

        GUILayout.Space(2);
        GUILayout.Label(node.TypeName, _titleStyle);

        int depCount = node.DependsOn.Count;
        int refCount = CountReferencesTo(node.TypeName);
        GUILayout.Label($"Deps: {depCount}  Refs: {refCount}", _subtitleStyle);

        var e = Event.current;
        if (e.type == EventType.MouseDown && e.clickCount == 2)
        {
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(node.FilePath);
            if (asset != null)
                AssetDatabase.OpenAsset(asset);
            e.Use();
        }

        GUI.DragWindow();
    }

    void DrawEdges()
    {
        if (_graph == null) return;

        Handles.BeginGUI();

        foreach (var node in _graph.Nodes)
        {
            var sourceRect = GetScreenRect(node);

            foreach (var depName in node.DependsOn)
            {
                if (!_graph.NodesByType.TryGetValue(depName, out var target))
                    continue;

                var targetRect = GetScreenRect(target);

                var startPoint = GetNearestEdgePoint(sourceRect, targetRect.center);
                var endPoint = GetNearestEdgePoint(targetRect, sourceRect.center);

                float dist = Vector2.Distance(startPoint, endPoint);
                float tangentStrength = Mathf.Clamp(dist * 0.3f, 20f, 80f);
                var startTangent = startPoint + GetEdgeDirection(sourceRect, startPoint) * tangentStrength;
                var endTangent = endPoint + GetEdgeDirection(targetRect, endPoint) * tangentStrength;

                var edgeColor = new Color(0.8f, 0.8f, 0.8f, 0.6f);
                Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, edgeColor, null, EdgeWidth);

                var arrowDir = (endPoint - endTangent).normalized;
                if (arrowDir.sqrMagnitude > 0.001f)
                    DrawArrowhead(endPoint, arrowDir, edgeColor);
            }
        }

        Handles.EndGUI();
    }

    void HandlePanning()
    {
        var e = Event.current;

        if (e.type == EventType.MouseDown && (e.button == 2 || (e.button == 0 && e.alt)))
        {
            _isPanning = true;
            _panStartMouse = e.mousePosition;
            _panStartOffset = _panOffset;
            e.Use();
        }

        if (e.type == EventType.MouseDrag && _isPanning)
        {
            _panOffset = _panStartOffset + (e.mousePosition - _panStartMouse);
            e.Use();
            Repaint();
        }

        if (e.type == EventType.MouseUp && _isPanning)
        {
            _isPanning = false;
            e.Use();
        }
    }

    void CenterGraph()
    {
        if (_graph == null || _graph.Nodes.Count == 0) return;

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;
        foreach (var node in _graph.Nodes)
        {
            if (node.Rect.x < minX) minX = node.Rect.x;
            if (node.Rect.y < minY) minY = node.Rect.y;
            if (node.Rect.xMax > maxX) maxX = node.Rect.xMax;
            if (node.Rect.yMax > maxY) maxY = node.Rect.yMax;
        }

        float graphWidth = maxX - minX;
        float graphHeight = maxY - minY;
        float viewWidth = position.width;
        float viewHeight = position.height - ToolbarHeight;

        _panOffset = new Vector2(
            (viewWidth - graphWidth) / 2f - minX,
            (viewHeight - graphHeight) / 2f - minY);
    }

    Rect GetScreenRect(ScriptNode node)
    {
        return new Rect(
            node.Rect.x + _panOffset.x,
            node.Rect.y + _panOffset.y + ToolbarHeight,
            node.Rect.width,
            node.Rect.height);
    }

    int CountReferencesTo(string typeName)
    {
        int count = 0;
        foreach (var node in _graph.Nodes)
        {
            if (node.DependsOn.Contains(typeName))
                count++;
        }
        return count;
    }

    static Vector2 GetNearestEdgePoint(Rect rect, Vector2 externalPoint)
    {
        var center = rect.center;
        var dir = externalPoint - center;

        if (dir.sqrMagnitude < 0.001f)
            return center;

        float halfW = rect.width / 2f;
        float halfH = rect.height / 2f;

        float scaleX = dir.x != 0 ? halfW / Mathf.Abs(dir.x) : float.MaxValue;
        float scaleY = dir.y != 0 ? halfH / Mathf.Abs(dir.y) : float.MaxValue;
        float scale = Mathf.Min(scaleX, scaleY);

        return center + dir * scale;
    }

    static Vector2 GetEdgeDirection(Rect rect, Vector2 edgePoint)
    {
        bool onLeft = Mathf.Abs(edgePoint.x - rect.xMin) < 1f;
        bool onRight = Mathf.Abs(edgePoint.x - rect.xMax) < 1f;
        bool onTop = Mathf.Abs(edgePoint.y - rect.yMin) < 1f;
        bool onBottom = Mathf.Abs(edgePoint.y - rect.yMax) < 1f;

        if (onLeft) return Vector2.left;
        if (onRight) return Vector2.right;
        if (onTop) return Vector2.up;
        if (onBottom) return Vector2.down;

        return (edgePoint - rect.center).normalized;
    }

    static void DrawArrowhead(Vector2 tip, Vector2 direction, Color color)
    {
        direction.Normalize();
        float rad = ArrowAngle * Mathf.Deg2Rad;

        var right = new Vector2(
            direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad),
            direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad));

        var left = new Vector2(
            direction.x * Mathf.Cos(-rad) - direction.y * Mathf.Sin(-rad),
            direction.x * Mathf.Sin(-rad) + direction.y * Mathf.Cos(-rad));

        Handles.color = color;
        Handles.DrawAAConvexPolygon(
            (Vector3)tip,
            (Vector3)(tip - right * ArrowSize),
            (Vector3)(tip - left * ArrowSize));
    }
}
