using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackButton { LEFTCLICK, SKILL01, SKILL02 }

public class StanceManager : MonoBehaviour
{
    [SerializeField] private List<StanceSkillBase> stances;
    [SerializeField] private List<Transform> stanceIcons;

    private int activeStanceIndex = 0;

    public int GetActiveStanceIndex() { return activeStanceIndex; }
    public List<StanceSkillBase> GetStances() { return stances; }
    public StanceSkillBase GetActiveStance() { return stances[GetActiveStanceIndex()]; }
    public int GetStanceNumber() { return stances.Count; }

    private void Start()
    {
        Initialize();
        ActivateWeapon(0);
    }

    private void Initialize()
    {
         GameManager.instance.InputManager.GetInputActions().Player.MeleeStance.performed += ChangeToMeleeStance;
         GameManager.instance.InputManager.GetInputActions().Player.RangedStance.performed += ChangeToRangedStance;
         GameManager.instance.InputManager.GetInputActions().Player.DefenseStance.performed += ChangeToDefenseStance;
         GameManager.instance.InputManager.GetInputActions().Player.MagicStance.performed += ChangeToMagicStance;
    }

    // Input actions delegate
    private void ChangeToMeleeStance(InputAction.CallbackContext context) { if (context.performed) SelectWeapon(0); }
    private void ChangeToRangedStance(InputAction.CallbackContext context) { if (context.performed) SelectWeapon(1); }
    private void ChangeToDefenseStance(InputAction.CallbackContext context) { if (context.performed) SelectWeapon(2); }
    private void ChangeToMagicStance(InputAction.CallbackContext context) { if (context.performed) SelectWeapon(3); }

    public void AttackTrigger(AttackButton _button)
    {
        GetActiveStance().AttackTrigger(_button);
    }

    public void AcitvateStance(int _stanceIndex)
    {
        GameManager.instance.HUDManager.SideIcon.ActivateIcon(_stanceIndex);
        stances[_stanceIndex].ActivateStance();
    }

    /// <summary>
    /// Get the current active weapon
    /// </summary>
    public GameObject GetActiveWeapon()
    {
        GameObject weapon = stances[activeStanceIndex].gameObject;
        for (int i = 0; i < stances.Count; i++)
        {
            if (stances[i].gameObject.activeSelf == true) weapon = stances[i].gameObject;
        }
        return weapon;
    }

    /// <summary>
    /// Deactivates every weapon (before activate one)
    /// </summary>
    public void DeactivateEveryWeapon()
    {
        for (int i = 0; i < stances.Count; i++)
        {
            stances[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Selects the weapon (stance) and activates visual effects
    /// </summary>
    private void SelectWeapon(int _index)
    {
        if (GameManager.instance.ModularController.IsAttacking) return;

        GameManager.instance.BuffManager.BaseGemBuffTrigger(GetActiveStanceIndex(), BuffTrigger.ActivateSkillPage);

        if (activeStanceIndex == _index) return;

        if (!stances[_index].GetIsStanceActive()) return;

        if (transform.childCount <= _index) return;

        switch (_index)
        {
            case 0:
                GameManager.instance.BuffManager.TriggerMeleeStanceBuff();
                break;
        }

        activeStanceIndex = _index;

        GameManager.instance.HUDManager.SideIcon.ChangeSelectedIcon(_index);
        ActicateStanceIcon(_index);
        ActivateWeapon(_index);
    }

    /// <summary>
    /// Activates the weapon gameObject
    /// </summary>
    private void ActivateWeapon(int _index)
    {
        DeactivateEveryWeapon();
        ActicateStanceIcon(_index);
        stances[_index].gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables all stance icon
    /// </summary>
    private void DisableAllStanceIcon()
    {
        for (int i = 0; i < stanceIcons.Count; i++)
        {
            DisableIcon(i);
        }
    }

    private void DisableIcon(int _index)
    {
        stanceIcons[_index].gameObject.SetActive(false);
    }

    /// <summary>
    /// Activate the current active stance icon
    /// </summary>
    private void ActicateStanceIcon(int _index)
    {
        if (stanceIcons.Count <= _index) return;

        DisableAllStanceIcon();

        stanceIcons[_index].gameObject.SetActive(true);
    }
}

