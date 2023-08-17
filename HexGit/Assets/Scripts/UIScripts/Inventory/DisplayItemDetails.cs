using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayItemDetails : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescribe;
    [SerializeField] private RectTransform detailPopUpScreen;

    bool showDetails = false;

    private void Update()
    {
        ShowUpdate();
    }

    /// <summary>
    /// Display the name and description
    /// </summary>
    public void DisplayThisItem(int rarityIndex, string _itemName, string _itemDescribe)
    {
        itemName.text = _itemName;
        itemName.color = GameManager.instance.HUDManager.InventoryItems.GetRarityColor(rarityIndex);
        itemDescribe.text = _itemDescribe;
    }

    /// <summary>
    /// This UI element position
    /// </summary>
    public void DisplayPosition(Vector3 _position)
    {
        showDetails = true;
        transform.position = _position;
    }

    /// <summary>
    /// Activate and deactivate the UI element
    /// </summary>
    private void ShowUpdate()
    {
        if (showDetails)
        {
            detailPopUpScreen.gameObject.SetActive(true);
        }
        else
        {
            detailPopUpScreen.gameObject.SetActive(false);
        }

        showDetails = false;
    }
}
