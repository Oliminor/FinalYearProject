using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(PathFinder)), CanEditMultipleObjects]
public class PathFinderEditor : Editor
{
    PathFinder pathFinder;

    private void OnEnable()
    {
        pathFinder = (PathFinder)target;
        pathFinder.AstarTest();
    }

    private void OnDisable()
    {
        pathFinder.EraseLine();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Buttons setup
        GUILayout.Space(15);
        if (GUILayout.Button("Create Grid")) pathFinder.InstantiateGrid();
        if (GUILayout.Button("Astar Test")) pathFinder.AstarTest();
        if (GUILayout.Button("Generate Cubes")) pathFinder.GenerateNodeCubes();
        if (GUILayout.Button("Delete Cubes")) pathFinder.DeletePrimitives();
    }

    protected virtual void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {

        }

        //HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
}
#endif