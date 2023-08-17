using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabList;
    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject combineParent;
    [SerializeField] int chucnkSize;
    [SerializeField] int objectNumberSize;

    [HideInInspector] [SerializeField] int density;
    [HideInInspector] [SerializeField] int paletteIndex;
    [HideInInspector] [SerializeField] float spawnGap;
    [HideInInspector] [SerializeField] float radius;
    [HideInInspector] [SerializeField] float heightMin;
    [HideInInspector] [SerializeField] float heightMax;
    [HideInInspector] [SerializeField] float flowerScaleMin;
    [HideInInspector] [SerializeField] float flowerScaleMax;
    [HideInInspector] [SerializeField] bool alignToNormal;
    [HideInInspector] [SerializeField] bool isNotGrass;

    public bool GetAlignToNormal() { return alignToNormal; }
    public void SetAlignToNormal(bool _alignToNormal) { alignToNormal = _alignToNormal; }
    public bool GetIsNotGrass() { return isNotGrass; }
    public void SetIsNotGrass(bool _isNotGrass) { isNotGrass = _isNotGrass; }
    public int GetDensity() { return density; }
    public int GetPaletteIndex() { return paletteIndex; }
    public void SetPaletteIndex(int _pIndex) { paletteIndex = _pIndex; }
    public void SetDensity(int _density) { density = _density; }
    public float GetRadius() { return radius; }
    public void SetRadius(float _radius) { radius = _radius; }
    public float GetHeightMin() { return heightMin; }
    public void SetHeightMin(float _heightMin) { heightMin = _heightMin; }
    public float GetHeightMax() { return heightMax; }
    public void SetHeightMax(float _heightMax) { heightMax = _heightMax; }
    public float GetFlowerScaleMin() { return flowerScaleMin; }
    public void SetFlowerScaleMin(float _flowerScaleMin) { flowerScaleMin = _flowerScaleMin; }
    public float GetFlowerScaleMax() { return flowerScaleMax; }
    public void SetFlowerScaleMax(float _flowerScaleMax) { flowerScaleMax = _flowerScaleMax; }
    public float GetSpawnGap() { return spawnGap; }
    public void SetSpawnGap(float _spawnGap) { spawnGap = _spawnGap; }
    public List<GameObject> GetPrefabList() { return prefabList; }

    /// <summary>
    /// Combines the same type (Material) of meshes together using chunk system
    /// </summary>
    public void CombineObjects()
    {
        Dictionary<Vector3Int, List<GameObject>> foliageChunks = new();

        Material originalMat = parentObject.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial; // Get the material from the object
        string combineName = parentObject.transform.GetChild(0).name; // Get the name of the object (same as the original)


        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            if (parentObject.transform.GetChild(i).name != combineName) continue;

            // Chunk them, so it is not going to end up a one big mesh, but smaller parts
            int x = (int)parentObject.transform.GetChild(i).transform.position.x / chucnkSize;
            int y = (int)parentObject.transform.GetChild(i).transform.position.y / chucnkSize;
            int z = (int)parentObject.transform.GetChild(i).transform.position.z / chucnkSize;

            Vector3Int chunk = new Vector3Int(x, y, z);

            GameObject gameObject = parentObject.transform.GetChild(i).gameObject;

            // Sorting out the objects into chunks inside the Dictionary
            if (!foliageChunks.ContainsKey(chunk))
            {
                List<GameObject> value = new();

                value.Add(gameObject);

                foliageChunks.Add(chunk, value);
            }
            else
            {
                foliageChunks[chunk].Add(gameObject);
            }
        }

        foreach (KeyValuePair<Vector3Int, List<GameObject>> chunk in foliageChunks)
        {
            CombineInstance[] combine = new CombineInstance[chunk.Value.Count];

            // Calculating the middle middle point (pivot) of the objects
            List <Vector3> positionList = new();

            for (int i = 0; i < chunk.Value.Count; i++) positionList.Add(chunk.Value[i].transform.position);

            Vector3 pivotPoint = GetMiddlePoint(positionList);

            // Add the meshes vertex positions together inside an array
            for (int i = 0; i < chunk.Value.Count; i++)
            {
                combine[i].mesh = chunk.Value[i].GetComponent<MeshFilter>().sharedMesh;
                chunk.Value[i].transform.position -= pivotPoint;
                combine[i].transform = chunk.Value[i].transform.localToWorldMatrix;
            }

            // Combine the meshes together
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.CombineMeshes(combine);

            // Save the mesh out as an asset
            SaveMesh(mesh, combineName);

            // Add the Mesfilter and Mesrenrers to the new object so it will work and looks just like before
            GameObject combinedObject = new GameObject(combineName);
            combinedObject.transform.position = pivotPoint;
            combinedObject.AddComponent<MeshFilter>();
            combinedObject.AddComponent<MeshRenderer>();
            combinedObject.AddComponent<PositionHex>(); // Interactive grass script
            combinedObject.GetComponent<MeshFilter>().sharedMesh = mesh;
            combinedObject.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
            combinedObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if (combineParent) combinedObject.transform.parent = combineParent.transform;
            combinedObject.isStatic = true;
        }
        // deletes the individual meshes after combined
        DeleteAllPrefabs(combineName);

        //If there are any other type of objects, it will start again and combine those together too, until finished
        if (parentObject.transform.childCount > 0) CombineObjects();
    }

    // This function doesn't work with combined obvjects
    /// <summary>
    /// Removes the prefabs from the scene (only the same type as the selected)
    /// </summary>
    public void Eraser(float _radius, string _prefabName)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Collider[] grassTransforms = Physics.OverlapSphere(hitInfo.point, _radius);

            for (int i = 0; i < grassTransforms.Length; i++)
            {
                if (grassTransforms[i].transform.tag == "Foliage" && grassTransforms[i].name == _prefabName) DestroyImmediate(grassTransforms[i].gameObject);
            }
        }
    }

    /// <summary>
    /// Spawns the prefab on the ground
    /// </summary>
    public void SpawnPrefabs(GameObject _prefab, Vector3 _center, float _radius, float _scaleMin, float _scaleMax, float _spawnGap, float _flowerScaleMin, float _flowerScaleMax , int _spawnDensity, bool _alignToNormal, bool _isNotGrass)
    {
        // Density is the amount of prefabs spawned at each frame
        GameObject centerObject = this.gameObject;
        int density = (int)(Mathf.Pow(_radius, 2) * _spawnDensity);

        
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // Get the object under the mouse (Only draws on the selected mesh, to prevent accidents like painting on the wall instead of the ground)
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            centerObject = hitInfo.transform.gameObject;
        }

        List<Vector3> instantiateTransforms = new();

        for (int i = 0; i < density; i++)
        {
            // Grab random position
            Vector2 randomPos2D = Random.insideUnitCircle * _radius;

            Vector3 randomPos = new Vector3(randomPos2D.x, 0, randomPos2D.y) + _center;

            Vector2 checkPoint = HandleUtility.WorldToGUIPoint(randomPos);

            Ray checkRay = HandleUtility.GUIPointToWorldRay(checkPoint);

            if (Physics.Raycast(checkRay, out RaycastHit hitInfo2))
            {
                if (hitInfo2.transform.gameObject == centerObject)
                {
                    // If other prefabs are too close, this OverlapSpehere find them
                    Collider[] nearbyTransformsN = Physics.OverlapSphere(hitInfo2.point, _spawnGap);

                    bool isSkip = false;
                    
                    for (int k = 0; k < nearbyTransformsN.Length; k++)
                    {
                        if (nearbyTransformsN[k].transform.tag == "Foliage" && nearbyTransformsN[k].name == _prefab.name)
                        {
                            isSkip = true;
                            break;
                        }
                    }

                    if (isSkip) continue;

                    int counter = 0;

                    // Another too close check, but this one between the for loop elements, not the already placed one
                    for (int j = 0; j < instantiateTransforms.Count; j++)
                    {
                        float distance = Vector3.Distance(instantiateTransforms[j], hitInfo2.point);
                        if (distance < _spawnGap) counter++;
                        if (counter > 0) break;
                    }

                    if (counter > 0) continue;

                    // If every check false it will Instantiates the elements with custom settings (angle, scale, height and such)
                    GameObject go = PrefabUtility.InstantiatePrefab(_prefab, parentObject.transform) as GameObject;
                    go.transform.position = hitInfo2.point;
                    go.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 90), 0));
                    go.transform.localScale = new Vector3(1, Random.Range(_scaleMin, _scaleMax), 1);
                    if (_alignToNormal) go.transform.localRotation = Quaternion.FromToRotation(transform.up, hitInfo2.normal) * go.transform.rotation;
                    if (_isNotGrass)
                    {
                        float flowerScaleRange = Random.Range(_flowerScaleMin, _flowerScaleMax);
                        go.transform.localScale = new Vector3(flowerScaleRange, flowerScaleRange, flowerScaleRange);
                        go.transform.localPosition += new Vector3(0, Random.Range(_scaleMin, _scaleMax), 0);
                    }

                    instantiateTransforms.Add(hitInfo2.point);
                }
            }
        }
        // Set it Dirty, so the scene is savable, otherwise possibly lost every work related to this editor script
        EditorUtility.SetDirty(centerObject);

        if (parentObject.transform.childCount > objectNumberSize) CombineObjects();
    }

    /// <summary>
    /// Deletes every non-combined prefabs
    /// </summary>
    public void DeleteAllPrefabs()
    {
        int childNumber = parentObject.transform.childCount;

        for (int i = childNumber - 1; i >= 0; i--)
        {
            DestroyImmediate(parentObject.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Deletes every non-combined projects by name
    /// </summary>
    public void DeleteAllPrefabs(string _name)
    {
        int childNumber = parentObject.transform.childCount;

        for (int i = childNumber - 1; i >= 0; i--)
        {
            if (parentObject.transform.GetChild(i).name != _name) continue;
            DestroyImmediate(parentObject.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    ///  Emergency function (Don't use this)
    /// </summary>
    public void SaveAlreadyCombinedMeshes()
    {
        for (int i = 0; i < combineParent.transform.childCount; i++)
        {
            Mesh saveThisMeshOut = combineParent.transform.GetChild(i).gameObject.GetComponent<MeshFilter>().sharedMesh;
            string meshName = combineParent.transform.GetChild(i).name;
            SaveMesh(saveThisMeshOut, meshName);
        }
    }

    /// <summary>
    /// Saves the mesh inside the Asset folder 
    /// </summary>
    public void SaveMesh(Mesh _mesh, string _name)
    {
        //string path = EditorUtility.SaveFilePanel("Saving Mesh to", "Assets/Mesh/Foliage/SavedMesh/", _name, "asset");
        // if (string.IsNullOrEmpty(path)) return;
        //uniqueFileName = FileUtil.GetProjectRelativePath(uniqueFileName);

        string path = "Assets/Mesh/Foliage/SavedMesh/";
        string uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(path + _name.ToString() + ".asset");

        if (string.IsNullOrEmpty(uniqueFileName)) return;

        AssetDatabase.CreateAsset(_mesh, uniqueFileName);
    }

    /// <summary>
    /// Get the selected objects middle (pivot) point
    /// </summary>
    public Vector3 GetMiddlePoint(List<Vector3> _list)
    {
        Vector3 totalV3 = Vector3.zero;

        for (int i = 0; i < _list.Count; i++) totalV3 += _list[i];

        Vector3 centerPoint = totalV3 / _list.Count;

        return centerPoint;
    }

    /// <summary>
    /// Get the mouse world position
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

    /// <summary>
    ///  Get the normal angle under the mouse
    /// </summary>
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
}
#endif