using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GemIcon : BaseIconUI, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 pickUpPosition;
    Transform contentParent;
    Transform pickUpParent;
    bool resizeBool;

    void Start()
    {
        contentParent = GameManager.instance.HUDManager.InventoryItems.GetGemInventoryContent().transform;
    }

    /// <summary>
    /// Picks up the icon from anywhere if hold the left mouse button on it
    /// </summary>
    public void OnBeginDrag(PointerEventData pointerEventData)
    {

        pickUpParent = transform.parent;
        pickUpPosition = transform.position;

        resizeBool = false;

        UpdateSlot();

        // Creates a clone from the stack and removes one quanity from it
        if (GetQuantity() > 1)
        {
            GameObject go = Instantiate(gameObject, pickUpParent);
            go.GetComponent<BaseIconUI>().SetQuantity(GetQuantity() - 1);
            go.GetComponent<BaseIconUI>().UpdateIcon();
            go.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        // Moves the moving card to a different parent and changes sort order (no overlap issue)
        gameObject.GetComponent<Canvas>().overrideSorting = true;
        gameObject.GetComponent<Canvas>().sortingOrder += 1;


        transform.parent = GameManager.instance.HUDManager.InventoryItems.transform;

        // Set the moving quantity to 1
        SetQuantity(1);
        UpdateIcon();
    }

    /// <summary>
    /// Moves the gem card and makes its smaller if its above the gem slots (visually)
    /// </summary>
    public void OnDrag(PointerEventData pointerEventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        bool isOnGemResize = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == "GemResize") isOnGemResize = true;
        }
        if (!resizeBool && isOnGemResize)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            resizeBool = true;
        }
        if (!isOnGemResize && resizeBool)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(140, 186);
            resizeBool = false;
        }
        transform.position = pointerEventData.position;
    }

    /// <summary>
    /// Put the card down 
    /// </summary>
    public void OnEndDrag(PointerEventData pointerEventData)
    {
        gameObject.GetComponent<Canvas>().sortingOrder -= 1;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        foreach (RaycastResult result in results)
        {
            // If there is a card already there, it will switch the positions (old card position become the new card pick up position)
            if (result.gameObject.tag == "Icon" && result.gameObject != gameObject)
            {
                if (result.gameObject.transform.parent == contentParent)
                {
                    GameManager.instance.HUDManager.InventoryItems.CheckInventoryForStack(this);
                    return;
                }

                transform.parent = result.gameObject.transform.parent;
                result.gameObject.transform.position = pickUpPosition;
                result.gameObject.transform.parent = pickUpParent;

                if (result.gameObject.transform.parent == contentParent) GameManager.instance.HUDManager.InventoryItems.CheckInventoryForStack(result.gameObject.GetComponent<BaseIconUI>());

                GameManager.instance.HUDManager.StanceManagerUI.CheckGemSlots();
                return;
            }
        }
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag == "GemSocket")
            {
                transform.parent = result.gameObject.transform;
                GameManager.instance.HUDManager.StanceManagerUI.CheckGemSlots();
                return;
            }
        }
               GameManager.instance.HUDManager.InventoryItems.CheckInventoryForStack(this);
    }

}
