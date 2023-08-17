using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashManager : MonoBehaviour
{
    List<Transform> slotList = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        LoadSlots();
        GameManager.playerStash = this;
    }

    /// <summary>
    /// Checks if the stash contains certain object (tag)
    /// </summary>
    public Transform CheckSlots(string _tag)
    {
        Transform slotTransform = transform;

        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].childCount > 0)
            if (slotList[i].GetComponentsInChildren<Transform>()[1].tag == _tag)
            {
                slotTransform = slotList[i].GetComponentsInChildren<Transform>()[1].transform;
                break;
            }
        }

        return slotTransform;
    }

    /// <summary>
    /// returns the free slot number
    /// </summary>
    public int FreeSlotNumber()
    {
        int slotN = 0;

        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].childCount == 0) slotN++;
        }

        return slotN;
    }

    /// <summary>
    /// returns the free slot transform
    /// </summary>
    public Transform FreeSlotTransform()
    {
        Transform freeT = transform;
       
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].childCount == 0)
            {
                freeT = slotList[i];
                break;
            }
        }

        return freeT;
    }

    /// <summary>
    /// Loads in the slot transforms
    /// </summary>
    private void LoadSlots()
    {
        for (int i = 0; i < transform.childCount; i++) slotList.Add(GetComponentsInChildren<Transform>()[i + 1]);
    }
}
