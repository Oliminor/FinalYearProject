using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcDialogue : MonoBehaviour
{
    [TextArea(5, 15)]
    [SerializeField]  List<string> dialouges = new List<string>();
    private Transform player;
    bool isPlayerNearby = false;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameManager.instance.ModularController.transform;
         GameManager.instance.InputManager.GetInputActions().UI.Interact.performed += TriggerDialogue;
    }

    /// <summary>
    /// Starts the dialogue if the player close enough to the NPC and press the interact button
    /// </summary>
    private void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (player.GetComponent<ModularController>().GetCharacterStatus() != playerStatus.GROUND) return;

        if (!isPlayerNearby) return;

        isPlayerNearby = false;

        GameManager.instance.HUDManager.DialogueManager.SetDialogueList(ref dialouges);
        GameManager.instance.HUDManager.DisablePressToInteract();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject ==  GameManager.instance.ModularController.gameObject)
        {
            GameManager.instance.HUDManager.ShowPressToInteract("Press F to Talk");
            isPlayerNearby = true;
        }
    }

    // When the player left the collider, disables the interact UI element
    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.HUDManager.DisablePressToInteract();
        isPlayerNearby = false;
    }
}
