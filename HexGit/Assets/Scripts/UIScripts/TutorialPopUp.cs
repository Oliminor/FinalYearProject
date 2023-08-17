using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPopUp : MonoBehaviour
{
    [TextArea(5, 15)]
    [SerializeField] string tutorialText;

    bool isCharacterOnTheSymbol;
    bool isMessageIsOn;

    private void Start()
    {
         GameManager.instance.InputManager.GetInputActions().UI.Interact.performed += TriggerMessageBoard;
    }

    /// <summary>
    /// Triggers the message board if the player is on the symbol
    /// </summary>
    private void TriggerMessageBoard(InputAction.CallbackContext context)
    {
        if (!isCharacterOnTheSymbol) return;

        GameManager.instance.HUDManager.DisablePressToInteract();
        isMessageIsOn = !isMessageIsOn;

        if (isMessageIsOn) GameManager.instance.HUDManager.ShowMessageBoard(tutorialText);
        else GameManager.instance.HUDManager.DisableMessageBoard();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject ==  GameManager.instance.ModularController.gameObject)
        {
            isCharacterOnTheSymbol = true;
            GameManager.instance.HUDManager.ShowPressToInteract("Press F to Interact");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.HUDManager.DisablePressToInteract();
        GameManager.instance.HUDManager.DisableMessageBoard();

        isCharacterOnTheSymbol = false;
        isMessageIsOn = false;
    }
}
