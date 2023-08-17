using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemAquiredManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform parentTransform;
    private List<GameObject> QueueList = new();
    private int activeNotification;

    IEnumerator notiPopUpE;

    /// <summary>
    /// Instantiate the item acquired UI element 
    /// </summary>
    public void InstantiateNotification(string name, int rarityIndex)
    {
        GameObject go = Instantiate(notificationPrefab, parentTransform);
        go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;
       go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = GameManager.instance.HUDManager.InventoryItems.GetRarityColor(rarityIndex);
        go.SetActive(false);
        QueueList.Add(go);

        if (notiPopUpE != null) return;
        notiPopUpE = NotificationPopUp();
        StartCoroutine(notiPopUpE);
    }

    // Only activates 4 pop up text and the same time, the rest in put in Queue
    IEnumerator NotificationPopUp()
    {
        if (activeNotification >= 4) yield break;
        if (QueueList.Count == 0) yield break;

        activeNotification++;

        GameObject go = QueueList[0];
        QueueList.RemoveAt(0);

        go.SetActive(true);
        go.transform.SetSiblingIndex(0); // Sets the sibling index to 0 so the new pop Up text is always on the top

        yield return new WaitForSeconds(0.15f);

        StartCoroutine(NotificationPopUp()); // if queue not empty, pops up the new element

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(NotificationPopUp()); // if queue not empty, pops up the new element
        Destroy(go);
        activeNotification--;

        notiPopUpE = null;
    }
}
