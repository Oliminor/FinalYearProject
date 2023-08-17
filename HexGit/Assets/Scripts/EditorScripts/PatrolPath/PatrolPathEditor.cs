using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(PatrolPath)), CanEditMultipleObjects]
public class PatrolPathEditor : Editor
{
    PatrolPath patrolPath;

    private void OnEnable()
    {
        patrolPath = (PatrolPath)target;
        patrolPath.DrawLineBetweenPaths();
    }

    private void OnDisable()
    {
       patrolPath.HideLineRendererLines();
    }

    public override void OnInspectorGUI()
    {
        // Setup clearn button
        base.OnInspectorGUI();
        GUILayout.Space(15);
        if (GUILayout.Button("ClearPath")) patrolPath.ClearPathList();
    }

    protected virtual void OnSceneGUI()
    {
        // Clicking anywhere on a mesh (with collider) add that position to the path List
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            patrolPath.AddPathPoint(patrolPath.GetMousePosition() + Vector3.up * 2);
            patrolPath.DrawLineBetweenPaths();
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
}
#endif