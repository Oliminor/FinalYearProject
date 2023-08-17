using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions inputActions;

    private bool isLeftClickOnHold = false;

    public bool GetIsLeftClickOnHold() { return isLeftClickOnHold; }

    public InputActions GetInputActions() { return inputActions; }

    void Awake()
    {
        inputActions = new InputActions();
        EnablePlayerInputActions();
        EnableUIInputActions();

        inputActions.UI.LeftClick.performed += IsLeftClickHold;
        inputActions.UI.LeftClick.canceled += IsLeftClickHold;
    }

    /// <summary>
    /// Input vector calculation
    /// </summary>
    public Vector2 GetInputVector()
    {
        float h = GameManager.instance.InputManager.GetInputActions().Player.Move.ReadValue<Vector2>().x;
        float v = GameManager.instance.InputManager.GetInputActions().Player.Move.ReadValue<Vector2>().y;

        Vector2 inputVector = new Vector2(h, v);
        if (inputVector.magnitude > 1) inputVector = inputVector.normalized;

        return inputVector;
    }

    /// <summary>
    /// Returns false if the the player doesn't press any movement input
    /// </summary>
    public bool InputCheck()
    {
        float h = GameManager.instance.InputManager.GetInputActions().Player.Move.ReadValue<Vector2>().x;
        float v = GameManager.instance.InputManager.GetInputActions().Player.Move.ReadValue<Vector2>().y;

        if (h != 0 || v != 0) return true;
        return false;
    }

    /// <summary>
    /// Returns bool based on the left mouse button is hold or not
    /// </summary>
    public void IsLeftClickHold(InputAction.CallbackContext context)
    {
        if (context.performed) isLeftClickOnHold = true;

        if (context.canceled) isLeftClickOnHold = false;
    }

    /// <summary>
    /// Enables the player input
    /// </summary>
    public void EnablePlayerInputActions()
    {
        inputActions.Player.Enable();
    }

    /// <summary>
    /// Enables the UI input
    /// </summary>
    public void EnableUIInputActions()
    {
        inputActions.UI.Enable();
    }

    /// <summary>
    /// Disables Player Input
    /// </summary>
    public void DisablePlayerInputActions()
    {
        inputActions.Player.Disable();
    }

    /// <summary>
    /// Enables Player Input
    /// </summary>
    public void DisableUIInputActions()
    {
        inputActions.UI.Disable();
    }

    /// <summary>
    /// Toggles both Input to either on or off if needed
    /// </summary>
    public void ToggleInputs(bool _bool)
    {
        if (_bool)
        {
            inputActions.Player.Enable();
            inputActions.UI.Enable();
        }
        else
        {
            inputActions.Player.Disable();
            inputActions.UI.Disable();
        }
    }
}

