using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InventoryItems : MonoBehaviour
{
    [SerializeField] private GameObject stanceTab;
    [SerializeField] private GameObject skillTab;
    [SerializeField] private GameObject gemInventoryContent;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject itemDetails;
    [SerializeField] private RectTransform stanceIndexButtons;

    [SerializeField] private List<Color> rarityColorList = new List<Color>();

    [SerializeField] private GameObject defaultGem;
    [SerializeField] private GameObject defaultSkill;
    [SerializeField] private GameObject defaultBuff;

    private int stanceIndex;

    private bool isMouseActive = false;

    public GameObject GetGemInventoryContent() { return gemInventoryContent; }
    public Color GetRarityColor(int _index) { return rarityColorList[_index]; }
    public int GetStanceIndex() { return stanceIndex; }

    public GameObject GetDefaultGem() { return defaultGem; }
    public GameObject GetDefaultSkill() { return defaultSkill; }
    public GameObject GetDefaultBuff() { return defaultBuff; }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Cursor.lockState = CursorLockMode.Locked;

        stanceIndexButtons.gameObject.SetActive(false);
        inventoryUI.SetActive(false);
        itemDetails.SetActive(true);

         GameManager.instance.InputManager.GetInputActions().UI.OpenStanceGems.performed += OpenInventory;
         GameManager.instance.InputManager.GetInputActions().UI.ShowCursor.performed += CursorMode;
    }

    /// <summary>
    /// Add gem to the inventory
    /// </summary>
    public void AddGem(Inventory _gem)
    {
        Transform parentObject = GetGemInventoryContent().transform;

        GameObject go = Instantiate(GetDefaultGem(), parentObject);
        go.GetComponent<BaseIconUI>().SetIconData(_gem);
        CheckInventoryForStack(go.GetComponent<BaseIconUI>());
    }

    /// <summary>
    /// Add RandomItem to the inventory 
    /// </summary>
    public Inventory DropRandomItem(ItemType _itemType)
    {
        int ItemIndex = GameManager.instance.DataManager.GetRandomItem(_itemType, out int rarity);

        Inventory newItem = new(0,0,0);
        newItem.quantity = 1;
        newItem.index = ItemIndex;
        newItem.rarity = rarity;

        AddGem(newItem);
        return newItem;
    }

    /// <summary>
    /// If the player drag and drops the card somewhere it will checks if the inventory slots already have that card and if yes, increases the quantity
    /// </summary>
    public void CheckInventoryForStack(BaseIconUI _baseIcon)
    {
        if (_baseIcon.GetQuantity() > 1) return;

        for (int i = 0; i < gemInventoryContent.transform.childCount; i++)
        {
            BaseIconUI baseIconInventory = gemInventoryContent.transform.GetChild(i).gameObject.GetComponent<BaseIconUI>();

            if (_baseIcon.GetIndex() == baseIconInventory.GetIndex() && _baseIcon.GetRarityIndex() == baseIconInventory.GetRarityIndex())
            {
                baseIconInventory.SetQuantity(baseIconInventory.GetQuantity() + 1);
                baseIconInventory.UpdateIcon();
                GameManager.instance.HUDManager.StanceManagerUI.CheckGemSlots();
                Destroy(_baseIcon.gameObject);
            }
        }

        _baseIcon.gameObject.transform.parent = gemInventoryContent.transform;
        GameManager.instance.HUDManager.StanceManagerUI.CheckGemSlots();
    }

    /// <summary>
    /// Activates the new stance button when the player picks up new stance shard
    /// </summary>
    private void StanceButtonsActivate()
    {
        StanceManager playerStance = GameManager.instance.StanceManager;

        for (int i = 0; i < playerStance.GetStanceNumber(); i++)
        {
            if (playerStance.GetStances()[i].GetComponent<StanceSkillBase>().GetIsStanceActive()) stanceIndexButtons.GetChild(i).gameObject.SetActive(true);
            else stanceIndexButtons.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Loads up the new stance or skill tab (UI Button)
    /// </summary>
    public void SetStanceIndex(int _index) 
    { 
        stanceIndex = _index;
        if (stanceTab) GameManager.instance.HUDManager.StanceManagerUI.LoadGemPage(_index);
        if (skillTab) GameManager.instance.HUDManager.SkillManagerUI.LoadSkillPage(_index);
    }

    /// <summary>
    /// When the player press alt it activates the mouse and deactivates the keyboard input (and change it back)
    /// </summary>
    private void CursorMode(InputAction.CallbackContext context)
    {
        isMouseActive = !isMouseActive;

        if (inventoryUI.activeSelf == true)
        {
            isMouseActive = false;
            SetCameraAndModularController(false);
            return;
        }

        if (isMouseActive)
        {
            SetCameraAndModularController(false);
        }
        else
        {
            SetCameraAndModularController(true);
        }

    }

    /// <summary>
    /// Opens up the inventory
    /// </summary>
    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (inventoryUI.activeSelf == false)
        {
            SetCameraAndModularController(false);
            StanceButtonsActivate();
            inventoryUI.SetActive(true);
            OpenStanceTab();
        }
        else
        {
            SetCameraAndModularController(true);
            inventoryUI.SetActive(false);
        }
    }

    /// <summary>
    /// Locks the player input and camera if the inventory is on
    /// </summary>
    private void SetCameraAndModularController(bool _bool)
    {
        if (_bool)
        {
            Cursor.lockState = CursorLockMode.Locked;
             GameManager.instance.InputManager.EnablePlayerInputActions();
            GameManager.instance.CameraManager.ActivateCameraMovement();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
             GameManager.instance.InputManager.DisablePlayerInputActions();
            GameManager.instance.CameraManager.DisableCameraMovement();
        }
    }

    /// <summary>
    /// Opens up the stance tab
    /// </summary>
    public void OpenStanceTab()
    {
        stanceIndexButtons.gameObject.SetActive(true);
        skillTab.SetActive(false);
        stanceTab.SetActive(true);
        GameManager.instance.HUDManager.StanceManagerUI.LoadGemPage(stanceIndex);
    }

    /// <summary>
    /// Opens up the skill tab
    /// </summary>
    public void OpenSkillTab()
    {
        stanceIndexButtons.gameObject.SetActive(true);
        stanceTab.SetActive(false);
        skillTab.SetActive(true);
        GameManager.instance.HUDManager.SkillManagerUI.LoadSkillPage(stanceIndex);
    }
}
