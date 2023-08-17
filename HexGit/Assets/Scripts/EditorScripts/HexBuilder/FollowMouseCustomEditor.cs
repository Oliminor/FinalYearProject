using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------

[CustomEditor(typeof(FollowMouseEditor)), CanEditMultipleObjects]
public class FollowMouseCustomEditor : Editor
{
    FollowMouseEditor mouse;

    static bool OverrideHex;
    static bool deleteHex;

    static float range;
    static float lux;

    bool isControlDown;
    bool isShiftDown;
    bool isAltDown;

    static Color colorA;
    static Color colorB;
    static float randomColorRange;
    static  int paletteIndex;


    private void OnEnable()
    {
        mouse = (FollowMouseEditor)target;
        colorA = mouse.GetColorA();
        colorB = mouse.GetColorB();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);


        if (GUILayout.Button("Spawn Hex Bases"))
        {
            mouse.SpawnBases();
        }

        if (GUILayout.Button("Change to Top View"))
        {
            var sceneView = SceneView.lastActiveSceneView;
            Vector3 camerePos = SceneView.lastActiveSceneView.camera.transform.position;
            sceneView.LookAtDirect(camerePos, Quaternion.Euler(90, 0, 0));
        }

        HexPalette();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Random Color (0 = disabled):");
        randomColorRange = EditorGUILayout.FloatField(randomColorRange);
        GUILayout.EndHorizontal();

        ColorGUI();

        GUILayout.Space(20);

        OverrideHex = GUILayout.Toggle(OverrideHex, "Override Hex");
        deleteHex = GUILayout.Toggle(deleteHex, "Delete Hex");

        mouse.SetDeleteHex(deleteHex);

        GUILayout.BeginHorizontal("box");

        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.Label("Light Range");
        range = GUILayout.HorizontalSlider(range, 0.0F, 500.0F);
        GUILayout.Space(15);
        range = EditorGUILayout.FloatField(range);
        GUILayout.Space(15);

        GUILayout.Label("Lux Strength");
        lux = GUILayout.HorizontalSlider(lux, 0.0F, 100.0F);
        GUILayout.Space(15);
        lux = EditorGUILayout.FloatField(lux);
        GUILayout.Space(15);

        mouse.ChangeLightSettings(colorA, range, lux);

    }

    private void HexPalette()
    {
        GUIContent[] paletteIcons = new GUIContent[mouse.GetHexList().Count];
        string[] paletteNames = new string[mouse.GetHexList().Count];

        for (int i = 0; i < paletteIcons.Length; i++)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(mouse.GetHexList()[i]);
            paletteIcons[i] = new GUIContent(mouse.GetHexList()[i].name, texture);
            paletteNames[i] = mouse.GetHexList()[i].name;
        }

        paletteIndex = GUILayout.SelectionGrid(paletteIndex, paletteIcons, 4, GUILayout.MaxHeight(50));
    }

    private void ColorGUI()
    {
        colorA = EditorGUILayout.ColorField("Color A", colorA);
        colorB = EditorGUILayout.ColorField("Color B", colorB);

        Color tempColorA = colorA;
        Color tempColorB = colorB;

        float randomColorValueR = Random.Range(-randomColorRange / 255, randomColorRange / 255);
        float randomColorValueG = Random.Range(-randomColorRange / 255, randomColorRange / 255);
        float randomColorValueB = Random.Range(-randomColorRange / 255, randomColorRange / 255);

        if (randomColorRange != 0)
        {
            tempColorA += new Color(randomColorValueR, randomColorValueG, randomColorValueB, 0);
            tempColorB += new Color(randomColorValueR, randomColorValueG, randomColorValueB, 0);
        }        

        if (mouse.GetSelectedHex() != null)
        mouse.ChangeColor(tempColorA, tempColorB);
    }

    private void PlaceOrSelectHex()
    {
        int hexMode = mouse.SelectHex(mouse.GetHexList()[paletteIndex]);

        switch (hexMode)
        {
            case 1:
                if (deleteHex) return;
                if (OverrideHex) ColorGUI();
                else
                {
                    colorA = mouse.GetColorA();
                    colorB = mouse.GetColorB();
                    range = mouse.GetLightRange();
                    lux = mouse.GetLightIntensity();
                }
                break;

            case 2:
                ColorGUI();
                break;
        }

    }

    protected virtual void OnSceneGUI()
    {
        // Mouse Position while moving on the Editor Scene
        if (Event.current.type == EventType.MouseMove)
        {
            GameManager.mousePosition = mouse.MousePosition();
            mouse.UpadteHexBase();
        }

        // Checks if Control hold down or not for Scaling Hex
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == (KeyCode.LeftControl)) isControlDown = true;

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == (KeyCode.LeftControl)) isControlDown = false;

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
           if (isControlDown) mouse.ScaleHex();
        }

        // Checks if Alt hold down or not for Moving Hex on the Y position
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == (KeyCode.LeftShift)) isShiftDown = true;

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == (KeyCode.LeftShift)) isShiftDown = false;

        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            if (isShiftDown) mouse.MoveHex();
        }

        // Place Hex down 
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 || Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            PlaceOrSelectHex();
            GameManager.mousePosition = mouse.MousePosition();
            mouse.UpadteHexBase();
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == (KeyCode.LeftAlt)) isAltDown = true;

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == (KeyCode.LeftAlt)) isAltDown = false;

        // Scroll
        if (Event.current.type == EventType.ScrollWheel && isAltDown)
        {
            if (Event.current.delta.y > 0) mouse.HeightValueChange(1);
            else mouse.HeightValueChange(0);
            mouse.UpadteHexBase();
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
 }

#endif