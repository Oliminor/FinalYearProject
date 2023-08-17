using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(PrefabSpawner))]
public class PrefabSpawnerEditor : Editor
{
    PrefabSpawner spawner;

    int counter;

    static int paletteIndex;
    static int density;
    static float spawnGap;
    static float radius;
    static float heightMin;
    static float heightMax;
    static float flowerScaleMin;
    static float flowerScaleMax;

    static bool alignToNormal;
    static bool isNotGrass;

    bool isControlDown;

    private void OnEnable()
    {
        spawner = (PrefabSpawner)target;
        radius = spawner.GetRadius();
        density = spawner.GetDensity();
        spawnGap = spawner.GetSpawnGap();
        heightMin = spawner.GetHeightMin();
        heightMax = spawner.GetHeightMax();
        alignToNormal = spawner.GetAlignToNormal();
        isNotGrass = spawner.GetIsNotGrass();
        paletteIndex = spawner.GetPaletteIndex();
        flowerScaleMin = spawner.GetFlowerScaleMin();
        flowerScaleMax = spawner.GetFlowerScaleMax();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(15);

        if (GUILayout.Button("Combine prefabs"))
        {
            spawner.CombineObjects();
        }

        if (GUILayout.Button("Delete prefabs"))
        {
            spawner.DeleteAllPrefabs();
        }

        alignToNormal = GUILayout.Toggle(alignToNormal, "Align the rotation to normal");
        spawner.SetAlignToNormal(isNotGrass);
        isNotGrass = GUILayout.Toggle(isNotGrass, "Set the settings to non-grass objects");
        spawner.SetIsNotGrass(isNotGrass);

        GUILayout.Space(15);
        GUILayout.Label("Spawn radius");
        radius = GUILayout.HorizontalSlider(radius, 0.0F, 2.5F);
        GUILayout.Space(15);
        spawner.SetRadius(radius);

        GUILayout.Label("Density");
        density = EditorGUILayout.IntField(density);
        spawner.SetDensity(density);

        GUILayout.Space(15);
        GUILayout.Label("space between objects");
        spawnGap = EditorGUILayout.FloatField(spawnGap);
        spawner.SetSpawnGap(spawnGap);

        GUILayout.Space(15);
        GUILayout.Label("Grass Height (Min - Max)");
        GUILayout.BeginHorizontal("Height Values");
        heightMin = EditorGUILayout.FloatField(heightMin);
        spawner.SetHeightMin(heightMin);
        heightMax = EditorGUILayout.FloatField(heightMax);
        spawner.SetHeightMax(heightMax);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        GUILayout.Label("Non-Grass Scale (Min - Max)");
        GUILayout.BeginHorizontal("Height Values");
        flowerScaleMin = EditorGUILayout.FloatField(flowerScaleMin);
        spawner.SetFlowerScaleMin(flowerScaleMin);
        flowerScaleMax = EditorGUILayout.FloatField(flowerScaleMax);
        spawner.SetFlowerScaleMax(flowerScaleMax);
        GUILayout.EndHorizontal();

        PrefabPalette();
    }

    private void PrefabPalette()
    {
        // Creates the name and Icon arrays
        GUIContent[] paletteIcons = new GUIContent[spawner.GetPrefabList().Count];
        string[] paletteNames = new string[spawner.GetPrefabList().Count];

        for (int i = 0; i < paletteIcons.Length; i++)
        {
            // Dd the textures and name to the icon
            Texture2D texture = AssetPreview.GetAssetPreview(spawner.GetPrefabList()[i]);
            paletteIcons[i] = new GUIContent(spawner.GetPrefabList()[i].name, texture);
            paletteNames[i] = spawner.GetPrefabList()[i].name;
        }
        // Save the selected index
        paletteIndex = GUILayout.SelectionGrid(paletteIndex, paletteIcons, 4, GUILayout.MaxHeight(50));

        spawner.SetPaletteIndex(paletteIndex);

        // Hardcode for personal use
        if (paletteIndex != 0) isNotGrass = true;
        else isNotGrass = false;
        spawner.SetIsNotGrass(isNotGrass);
    }

    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.cyan;

        Handles.DrawWireDisc(spawner.GetMousePosition(), spawner.GetMeshNormalAtMousePosition(), radius);

        // Checks if Control hold down
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == (KeyCode.LeftControl)) isControlDown = true;

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == (KeyCode.LeftControl)) isControlDown = false;

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            if (isControlDown) spawner.Eraser(radius, spawner.GetPrefabList()[paletteIndex].name);
        }

        counter++;

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 || Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (counter < 10) return;

            counter = 0;
            if (isControlDown) return;
            spawner.SpawnPrefabs(spawner.GetPrefabList()[paletteIndex], spawner.GetMousePosition(), radius, heightMin, heightMax, spawnGap, flowerScaleMin, flowerScaleMax, density, alignToNormal, isNotGrass);
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
}

#endif