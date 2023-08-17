using System.Collections;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------
public class HexBaseEditor : EditorWindow
{
    Object hexBase;

    [MenuItem("Window/HexBaseEditor")]


    public static void ShowWindow()
    {
        GetWindow<HexBaseEditor>("HexBaseEditor");
    }

    private void OnGUI()
    {
        GUILayout.Space(20);

        GUILayout.Label("Spawn terraces around the Arena", EditorStyles.label);

        hexBase = EditorGUILayout.ObjectField(hexBase, typeof(GameObject), true);

        GUILayout.Space(20);

        if (GUILayout.Button("Fill Hex Base Array"))
        {

        }
    }

    void Update()
    {
       
    }

}

#endif