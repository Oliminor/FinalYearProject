using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanceShard : MonoBehaviour
{
    [SerializeField] private int stanceID;
    [SerializeField] private string popUpText;
    [SerializeField] private string iconPath;
    [SerializeField] private GameObject pickUpEffect;
    [SerializeField] private List<GameObject> showItemList;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform ==  GameManager.instance.ModularController.transform)
        {
            Debug.Log("Shard picked up");
            GameManager.instance.HUDManager.ShowStanceAquiredPopUp();
            GameManager.instance.StanceManager.AcitvateStance(stanceID);
            GameManager.instance.HUDManager.StanceAcquiredPopUp.TriggerStanceAcquired(popUpText, iconPath);

            GameObject go = Instantiate(pickUpEffect, transform.position, Quaternion.identity);

            ShowObjects();

            Destroy(gameObject);
            Destroy(go, 2.0f);
        }
    }

    private void ShowObjects()
    {
        if (showItemList.Count == 0) return;

        foreach (var item in showItemList)
        {
            item.SetActive(true);
        }
    }
}
