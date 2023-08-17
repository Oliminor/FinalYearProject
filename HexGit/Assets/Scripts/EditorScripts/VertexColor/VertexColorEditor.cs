using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------

[CustomEditor(typeof(VertexColor)), CanEditMultipleObjects]
public class VertexColorEditor : Editor
{
    VertexColor vertex;
    Color vertexColor;
    float radius;

    private void OnEnable()
    {
        vertex = (VertexColor)target;
        radius = vertex.GetRadius();
        vertexColor = vertex.GetVertexColor();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(15);

        GUILayout.Label("Spawn radius");
        radius = GUILayout.HorizontalSlider(radius, 0.0F, 2.5F);
        GUILayout.Space(15);
        vertex.SetRadius(radius);

        GUILayout.Space(15);
        GUILayout.Label("Vertex Color");
        GUILayout.BeginHorizontal("Vertex Color");
        if (GUILayout.Button("Black")) vertexColor = Color.black;
        vertex.SetVertexColor(vertexColor);
        if (GUILayout.Button("White")) vertexColor = Color.white;
        vertex.SetVertexColor(vertexColor);
        GUILayout.EndHorizontal();

    }

    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.cyan;

        Handles.DrawWireDisc(vertex.GetMousePosition(), vertex.GetMeshNormalAtMousePosition(), radius);

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 || Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            vertex.ColorVertex(radius, vertexColor);
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
}

#endif