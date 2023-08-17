using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] private List<Vector3> pathList;

    public List<Vector3> GetPathList() { return pathList; }
    public void AddPathPoint(Vector3 newPathPoint) { pathList.Add(newPathPoint); }
#if UNITY_EDITOR
    public void ClearPathList() 
    { 
        pathList.Clear();
        HideLineRendererLines();
    }

    /// <summary>
    /// When the gameobject is not selected (inspector), the Line Renderer is disabled
    /// </summary>
    public void HideLineRendererLines()
    {
        LineRenderer lr = GetComponent<LineRenderer>();

        Vector3[] positionsEmpty = new Vector3[1] { transform.position };
        lr.positionCount = 1;
        lr.SetPositions(positionsEmpty);

        EditorUtility.SetDirty(lr);
    }

    /// <summary>
    /// Draws line between the path node elements
    /// </summary>
    public void DrawLineBetweenPaths()
    {
        LineRenderer lr = GetComponent<LineRenderer>();

        if (pathList.Count < 2)
        {
            Vector3[] positionsEmpty = new Vector3[1] { transform.position };
            lr.positionCount = 1;
            lr.SetPositions(positionsEmpty);
            return;
        }

        Vector3[] positions = new Vector3[pathList.Count];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = pathList[i];
        }
        lr.positionCount = positions.Length;
        lr.loop = true;
        lr.SetPositions(positions);

        EditorUtility.SetDirty(lr);
    }

    /// <summary>
    /// Get the world position of the mouse using raycast
    /// </summary>
    public Vector3 GetMousePosition()
    {

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }
#endif
}
