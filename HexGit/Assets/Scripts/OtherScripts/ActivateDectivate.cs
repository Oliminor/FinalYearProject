using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------
public class ActivateDectivate : MonoBehaviour
{
    [SerializeField] private List<Transform> activateList;
    [SerializeField] private List<Transform> deativateList;


    public void TriggerListElements()
    {
        foreach (var item in activateList)
        {
            item.gameObject.SetActive(true);
        }

        foreach (var item in deativateList)
        {
            item.gameObject.SetActive(false);
        }
    }
}
