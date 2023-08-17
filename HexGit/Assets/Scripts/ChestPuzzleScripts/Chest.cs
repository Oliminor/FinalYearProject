using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [SerializeField] private int randomGemNumber;

    [SerializeField] private List<GameObject> puzzleList = new List<GameObject>();
    [SerializeField] private GameObject chestOpenParticle;
    [SerializeField] private GameObject chestCollider;
    [SerializeField] private GameObject chestShield;
    [SerializeField] private GameObject chestShineEffect;
    [SerializeField] private GameObject chestShieldDestroyEffect;


    private Animator anim;
    private int puzzleN;
    private int solvedPuzzleN = 0;
    private bool isChestSolved = false;
    private bool isChestOpened = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        chestShineEffect.SetActive(false);
        if (puzzleList.Count == 0) ChestSolved();
        puzzleN = puzzleList.Count;

         GameManager.instance.InputManager.GetInputActions().UI.Interact.performed += OpenChest;
    }

    /// <summary>
    /// Called when part of the puzzle is completed (and if all completed, sets the chest to the Solved state (visual effects enabled))
    /// </summary>
    public void PuzzleSolved()
    {
        solvedPuzzleN++;
        if (puzzleN == solvedPuzzleN) ChestSolved();
    }

    /// <summary>
    /// Disables the locked Shader effect and Enables the unlocked chest particle effects
    /// </summary>
    private void ChestSolved()
    {
        chestShineEffect.SetActive(true);
        chestShineEffect.GetComponent<ParticleSystem>().Play();
        chestShield.GetComponent<MeshRenderer>().enabled = false;
        chestShield.GetComponent<CapsuleCollider>().enabled = false;
        chestShieldDestroyEffect.SetActive(true);
        chestShieldDestroyEffect.GetComponent<ParticleSystem>().Play();
        isChestSolved = true;
        GameManager.instance.HUDManager.ShowPressToInteract("Press F to Open Chest");

    }

    /// <summary>
    /// Drops N number of random items (gems so far) when the player opens it
    /// </summary>
    private void DropRandomItems()
    {
        for (int i = 0; i < randomGemNumber; i++)
        {
            Inventory item = GameManager.instance.HUDManager.InventoryItems.DropRandomItem(ItemType.Gem);
            string name = GameManager.instance.DataManager.GetItemDatabase()[item.index].name;
            GameManager.instance.HUDManager.ItemAquiredManager.InstantiateNotification(name, item.rarity);
        }
    }

    /// <summary>
    /// Disables every effect and plays the opening animation (and destroys if after)
    /// </summary>
    private void OpenChest(InputAction.CallbackContext context)
    {
        if (isChestOpened) return;

        float distance = Vector3.Distance(transform.position,  GameManager.instance.ModularController.transform.position);

        if (distance < 3.5f && isChestSolved)
        {
            DropRandomItems();
            chestCollider.GetComponent<SphereCollider>().enabled = false;
            anim.SetTrigger("Open");
            chestCollider.SetActive(false);
            chestShineEffect.SetActive(false);
            GameObject go = Instantiate(chestOpenParticle, chestCollider.transform.position, Quaternion.identity);
            Destroy(go, 0.7f);
            isChestOpened = true;
            GameManager.instance.HUDManager.DisablePressToInteract();
            Destroy(gameObject, 1.0f);
        }
    }

    // When the player intersect with the collider, shows the interact UI element
    private void OnTriggerEnter(Collider other)
    {
        if (isChestOpened) return;

        if (!isChestSolved) return;

        if (other.gameObject == GameManager.instance.ModularController.gameObject)
        {
            GameManager.instance.HUDManager.ShowPressToInteract("Press F to Open Chest");
        }
    }

    // When the player left the collider, disables the interact UI element
    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.HUDManager.DisablePressToInteract();
    }
}
