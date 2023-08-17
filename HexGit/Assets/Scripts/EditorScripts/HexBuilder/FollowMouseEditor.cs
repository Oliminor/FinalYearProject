using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;

#if UNITY_EDITOR

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------
public class FollowMouseEditor : MonoBehaviour
{
    [SerializeField] GameObject selectedHex;

    [SerializeField] GameObject hexBase;
    [SerializeField] int radius = 1;
    [SerializeField] int spawnHeight = 0;

    [SerializeField] List<GameObject> HexList;

    [SerializeField] List<GameObject> chunkList;

    [SerializeField] GameObject[] spawnableObjects;

    bool deleteHex;

    private int tempRadius;
    private int tempHeight;
    private float tempX;
    private float tempY;
    private float tempMouseY;

    public void SetDeleteHex(bool _bool) { deleteHex = _bool; }

    public List<GameObject> GetHexList() { return HexList; }
    public Color GetColorA()
    {
       if (selectedHex) return selectedHex.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_ColorA");
        return Color.black;
    }
    public Color GetColorB()
    {
        if (selectedHex)
        {
            if (selectedHex.transform.childCount < 1) return selectedHex.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_ColorB");
        }
        return Color.black;
    }
    public float GetLightRange() 
    {
       if (selectedHex.transform.childCount > 0) return selectedHex.GetComponentInChildren<HDAdditionalLightData>().range;
        return 0;
    }
    public float GetLightIntensity() 
    {
        if (selectedHex.transform.childCount > 0) return selectedHex.GetComponentInChildren<HDAdditionalLightData>().intensity;
        return 0;
    }
    public GameObject GetSelectedHex() { return selectedHex; }

    public Vector3 MousePosition()
    {


        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    public void ChangeColor(Color colorA, Color colorB)
    {
        MeshRenderer renderer = selectedHex.GetComponent<MeshRenderer>();
        Material tempMaterial = new Material(renderer.sharedMaterial);
        tempMaterial.SetColor("_ColorA", colorA);
        tempMaterial.SetColor("_ColorB", colorB);
        renderer.sharedMaterial = tempMaterial;
    }

    public void ChangeLightSettings (Color colorA, float range, float luxIntensity)
    {
        if (selectedHex == null) return;
        if (selectedHex.tag != "Hex") return;
        if (selectedHex.transform.childCount < 1) return;

        HDAdditionalLightData hdLight = selectedHex.GetComponentInChildren<HDAdditionalLightData>();

        hdLight.intensity = luxIntensity;
        hdLight.color = colorA;
        hdLight.range = range;
    }

    public int SelectHex(GameObject hex)
    {

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Hex")
            {
                selectedHex = hitInfo.transform.gameObject;
                //Selection.activeObject = selectedHex;

                if (deleteHex) DeleteHex();
                return 1;
            }
            if (hitInfo.transform.tag == "AddHex")
            {
                if (!deleteHex) selectedHex = InstantiateHex(hitInfo.transform.position, hex);
                return 2;
            }
        }
        return 0;
    }

    public void DeleteHex() 
    { 
        DestroyImmediate(selectedHex);

        for (int i = 0; i < chunkList.Count; i++)
        {
            if (chunkList[i].transform.childCount < 1)
            {
                DestroyImmediate(chunkList[i]);
                chunkList.Remove(chunkList[i]);
            }
        }
    }

    public void MoveHex()
    {
        if (selectedHex == null) return;
        if (selectedHex.tag != "Hex") return;

        float mouseY = Event.current.mousePosition.y;
        float distance = mouseY - tempMouseY;

        if (distance < 5 && distance > -5)
        selectedHex.transform.position = new Vector3(selectedHex.transform.position.x, selectedHex.transform.position.y - distance / 25, selectedHex.transform.position.z);

        if (selectedHex.transform.position.y < 0)
        {
            selectedHex.transform.position = new Vector3(selectedHex.transform.position.x, 0, selectedHex.transform.position.z);
        }

        Vector3 rup = selectedHex.transform.TransformDirection(Vector3.up);
        Vector3 rdown = selectedHex.transform.TransformDirection(Vector3.down);

        Vector3 startPosition = new Vector3(selectedHex.transform.position.x, selectedHex.transform.position.y + selectedHex.transform.GetComponent<MeshCollider>().bounds.size.y / 2, selectedHex.transform.position.z);

        bool hitBoolUp = false;
        bool hitBoolDown = false;

        float limitYUp = 0;
        float limitYDown = 0;

        if (Physics.Raycast(startPosition, rup, out RaycastHit checkUp))
        {
            if (checkUp.transform.CompareTag("Hex") && checkUp.transform.position != selectedHex.transform.position)
            {
                limitYUp = checkUp.transform.position.y - selectedHex.transform.GetComponent<MeshCollider>().bounds.size.y;
                hitBoolUp = true;
            }
        }

        if (Physics.Raycast(startPosition, rdown, out RaycastHit checkDown))
        {
            if (checkDown.transform.CompareTag("Hex") && checkDown.transform.position != selectedHex.transform.position)
            {
                limitYDown = checkDown.transform.position.y + checkDown.transform.GetComponent<MeshCollider>().bounds.size.y;
                hitBoolDown = true;
            }
        }

        if (selectedHex.transform.position.y > limitYUp && hitBoolUp)
        {
            selectedHex.transform.position = new Vector3(selectedHex.transform.position.x, limitYUp, selectedHex.transform.position.z);
        }

        if (selectedHex.transform.position.y < limitYDown && hitBoolDown)
        {
            selectedHex.transform.position = new Vector3(selectedHex.transform.position.x, limitYDown, selectedHex.transform.position.z);
        }
        tempMouseY = mouseY;
    }

    public void ScaleHex()
    {
        if (selectedHex == null) return;
        if (selectedHex.tag != "Hex") return;

        float mouseY = Event.current.mousePosition.y;
        float distance = mouseY - tempMouseY;
        float size = selectedHex.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
        bool isTopHalf = false;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Hex")
            {
                float rayY = ray.GetPoint(hitInfo.distance).y;
                float selectedY = selectedHex.transform.position.y;
                if (rayY > selectedY + (size / 2)) isTopHalf = true;
            }

        }

        if (distance < 5 && distance > -5 )
        {
            if (isTopHalf) selectedHex.transform.localScale = new Vector3(selectedHex.transform.localScale.x, selectedHex.transform.localScale.y - distance / 50, selectedHex.transform.localScale.z);
            else
            {
                selectedHex.transform.localScale = new Vector3(selectedHex.transform.localScale.x, selectedHex.transform.localScale.y + distance / 100, selectedHex.transform.localScale.z);
                selectedHex.transform.position = new Vector3(selectedHex.transform.position.x, selectedHex.transform.position.y - distance / 50, selectedHex.transform.position.z);
            }
        }

        if (selectedHex.transform.localScale.y < 0.3)
        {
            selectedHex.transform.localScale = new Vector3(selectedHex.transform.localScale.x, 0.3f, selectedHex.transform.localScale.z);
        }

        Vector3 rup = selectedHex.transform.TransformDirection(Vector3.up);

        Vector3 startPosition = new Vector3(selectedHex.transform.position.x, selectedHex.transform.position.y + selectedHex.transform.GetComponent<MeshCollider>().bounds.size.y / 2, selectedHex.transform.position.z);

        bool hitBoolUp = false;

        float limitYUp = 0;

        if (Physics.Raycast(startPosition, rup, out RaycastHit checkUp))
        {
            if (checkUp.transform.CompareTag("Hex") && checkUp.transform.position != selectedHex.transform.position)
            {
                limitYUp = (checkUp.transform.position.y - selectedHex.transform.position.y) / size - 0.01f;
                hitBoolUp = true;
            }
        }

        if ((selectedHex.transform.localScale.y) > limitYUp && hitBoolUp)
        {
            selectedHex.transform.localScale = new Vector3(selectedHex.transform.localScale.x, limitYUp, selectedHex.transform.localScale.z);
        }

        tempMouseY = mouseY;
    }

    public GameObject InstantiateHex(Vector3 position, GameObject hex)
    {
        GameObject go = null;
        int xCoord = (int)(position.x / 32);
        int zCoord = (int)(position.z / 32);

        string name = "Chunk" + xCoord.ToString() + "-" + zCoord.ToString();

        bool isMatch = false;
        GameObject parent = null;

        for (int i = 0; i < chunkList.Count; i++)
        {
            if (chunkList[i].name == name)
            {
                isMatch = true;
                parent = chunkList[i];
            }
        }

        if (!isMatch)
        {
            parent = new GameObject(name);
            chunkList.Add(parent);
        }

        go = PrefabUtility.InstantiatePrefab(hex, parent.transform) as GameObject;
        go.transform.position = position;
        go.gameObject.AddComponent(typeof(DeleteDuplicate));

        EditorUtility.SetDirty(go);

        return go;
    }

    public void SpawnBases()
    {
        for (int i = 0; i < spawnableObjects.Length; i++)
        {
            DestroyImmediate(spawnableObjects[i]);
        }

        float objectN = Mathf.Pow(2 * radius + 1, 2);
        spawnableObjects = new GameObject[(int)objectN];

        for (int j = 0; j < (int)objectN; j++)
        {
            spawnableObjects[j] = Instantiate(hexBase, new Vector3(0, 0, 0), Quaternion.identity, transform);
        }
    }

    public void HeightValueChange(int value)
    {
        if (value == 1) spawnHeight++;
        else spawnHeight--;
    }

    public void UpadteHexBase()
    {
        if (tempRadius != radius) SpawnBases();

        float xOffset = 1.73f;
        float zOffset = 1.5f;

        float centerX = (int)(GameManager.mousePosition.x / xOffset);
        float centerY = (int)(GameManager.mousePosition.z / zOffset);

        int counter = 0;

        spawnHeight = Mathf.Clamp(spawnHeight, 0, 300);

        if (tempX != centerX || tempY != centerY || tempHeight != spawnHeight)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    float xPos = x * xOffset;

                    if ((int)centerY % 2 == 0)
                    {
                        if (y % 2 == 1 || y % 2 == -1) xPos += xOffset / 2f;
                    }
                    else
                    {
                        if (y % 2 == 0) xPos -= xOffset / 2;
                    }
                    spawnableObjects[counter].transform.position = new Vector3((centerX * xOffset) + xPos, spawnHeight, (((int)centerY * zOffset) + y * zOffset));
                    counter++;
                }
            }
        }

        tempX = centerX;
        tempY = centerY;
        tempRadius = radius;
        tempHeight = spawnHeight;
    }
}
#endif