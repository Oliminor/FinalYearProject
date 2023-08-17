using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------
public class VertexColor : MonoBehaviour
{
    [HideInInspector] [SerializeField] float radius;
    [HideInInspector] [SerializeField] Color vertexColor;

    public float GetRadius() { return radius; }
    public void SetRadius(float _radius) { radius = _radius; }
    public Color GetVertexColor() { return vertexColor; }
    public void SetVertexColor(Color _color) { vertexColor = _color; }

#if UNITY_EDITOR
    public void ColorVertex(float _radius, Color _color)
    {
        GameObject selectedGo = this.gameObject;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            selectedGo = hitInfo.transform.gameObject;
        }

        Mesh mesh = null;

        if (selectedGo.TryGetComponent(out MeshFilter _meshFilter))
        {
            mesh = _meshFilter.sharedMesh;
        }

        if (!mesh) return;

        Vector3[] vertices = mesh.vertices;

        Color[] colors = new Color[vertices.Length];

        if (mesh.colors.Length == 0)
        {
            for (int i = 0; i < vertices.Length; i++) colors[i] = Color.white;

            mesh.colors = colors;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPt = selectedGo.transform.TransformPoint(mesh.vertices[i]);

            float distance = Vector3.Distance(hitInfo.point, worldPt);

            colors[i] = mesh.colors[i];

            if (distance <= _radius )
            {
                colors[i] = _color;
            }
        }
        mesh.colors = colors;

        EditorUtility.SetDirty(selectedGo);
    }

    public Vector3 GetMousePosition()
    {

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    public Vector3 GetMeshNormalAtMousePosition()
    {

        Vector3 slopeAngle = Vector3.zero;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            slopeAngle = hitInfo.normal;
        }
        return slopeAngle;
    }
#endif
}
