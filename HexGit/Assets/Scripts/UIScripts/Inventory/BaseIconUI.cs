using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class BaseIconUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject rarityBG;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI quantityNumber;

    [SerializeField][HideInInspector]Inventory iconData;

    private bool isPointerOn = false;
    private bool isGemBuff;

    public void SetIsGemBuff(bool _bool) { isGemBuff = _bool; } 
    public int GetIndex() { return iconData.index; }
    public Color GetRarityColor() { return GameManager.instance.HUDManager.InventoryItems.GetRarityColor(iconData.rarity); }
    public int GetRarityIndex() { return iconData.rarity; }
    public int GetQuantity() { return iconData.quantity; }
    public void SetQuantity(int _quantity) 
    { 
        iconData.quantity = _quantity;
        UpdateIcon();
    }

    public void SetIconData(ItemData _iconData)
    {
        iconData.index = _iconData.index;
        iconData.rarity = _iconData.rarity;
        iconData.quantity = 1;

        rarityBG.GetComponent<Image>().color = GetRarityColor();

        iconImage.sprite = Resources.Load<Sprite>(GameManager.instance.DataManager.GetIconPath(iconData.index));

        UpdateIcon();
    }

    public void SetIconData(Inventory _iconData)
    {
        iconData = _iconData;

        rarityBG.GetComponent<Image>().color = GetRarityColor();

        if (quantityNumber) quantityNumber.text = _iconData.quantity.ToString();

        iconImage.sprite = Resources.Load<Sprite>(GameManager.instance.DataManager.GetIconPath(iconData.index));

        UpdateIcon();
    }

    private void OnEnable()
    {
        UpdateIcon();
    }

    void Update()
    {
        UpdateScale();
        UpdateSlot();
    }

    /// <summary>
    /// Changes the icon appearance if the Quantity is 1 (no quantity number)
    /// </summary>
    private void UpdateQuantityNumber()
    {
        if (!rarityBG) return;
        if (!quantityNumber) return;

        if (iconData.quantity <= 1)
        {
            rarityBG.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            quantityNumber.gameObject.SetActive(false);
        }
        else
        {
            rarityBG.GetComponent<RectTransform>().offsetMin = new Vector2(0, 40);
            quantityNumber.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Updates the icon background colour and quantity number
    /// </summary>
    public void UpdateIcon()
    {
        rarityBG.GetComponent<Image>().color = GetRarityColor();
        if (quantityNumber) quantityNumber.text = iconData.quantity.ToString();
        UpdateQuantityNumber();
    }

    /// <summary>
    /// Changes the icon size when the Mouse is on and opens up the pop up UI for the item descreption
    /// </summary>
    public void UpdateSlot()
    {
        if (!isPointerOn) return;

        if ( GameManager.instance.InputManager.GetIsLeftClickOnHold()) return;

        transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

        DisplayItemDetails displayUI = GameManager.instance.HUDManager.DisplayItemDetails;

        string displayText = "";

        if (!isGemBuff)
        {
            displayText += GameManager.instance.DataManager.GetDescription(iconData.index, iconData.rarity, 0, 0) + "\n" + "\n";
            displayText += GameManager.instance.DataManager.GetDescription(iconData.index, iconData.rarity, 1, 0) + "\n" + "\n";
        }

        displayText += GameManager.instance.DataManager.GetDescription(iconData.index, iconData.rarity, 2, 0);


        // Calculating the pop up window position based on the screen position so it is always on the screen
        displayUI.DisplayThisItem(iconData.rarity, GameManager.instance.DataManager.GetIconName(iconData.index), displayText);

        Vector3 displayPos = transform.position;
        float offsetX = transform.GetComponent<RectTransform>().sizeDelta.x / 1.75f;
        float offsetY = transform.GetComponent<RectTransform>().sizeDelta.y / 2;
        Vector3 offSet = new Vector3(offsetX, offsetY, 0);
        Vector3 finalPos = displayPos + offSet;

        float width = displayUI.GetComponent<RectTransform>().sizeDelta.x;
        float height = displayUI.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;

        if (transform.position.x > Screen.width - width)
        {
            offSet.y = -offsetY;
            displayPos.x -= width;
            finalPos = displayPos - offSet;
        }

        if(transform.position.y < height)
        {
            offSet.x = -offsetX;
            displayPos.y -= displayPos.y - height - offsetY;
            finalPos = displayPos - offSet;
        }

        displayUI.DisplayPosition(finalPos);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isPointerOn = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isPointerOn = false;
    }

    /// <summary>
    /// Scale up the icon if it is selected
    /// </summary>
    private void UpdateScale()
    {
        if (!isPointerOn)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.2f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.05f, 1.05f, 1.05f), 0.2f);
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        UpdateSlot();
    }
}
