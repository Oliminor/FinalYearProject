using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StanceSkillBase : MonoBehaviour
{
    [SerializeField] bool isActive = false;

    [SerializeField] protected WeaponBase LeftClickSlot;
    [SerializeField] protected WeaponBase qSkillAttack;
    [SerializeField] protected WeaponBase eSkillAttack;

    [SerializeField] protected LayerMask target;
    [SerializeField] protected LayerMask solidObject;

    protected Animator anim;
    protected ModularController player;

    protected int index;
    public int GetIndex() { return index; }
    public void SetIndex(int _index) { index = _index; }
    public bool GetIsStanceActive() { return isActive; }
    public void ActivateStance() { isActive = true; }

    void Start()
    {
        Initalize();
    }

    public virtual void Initalize()
    {
        player =  GameManager.instance.ModularController;
        anim = player.GetComponent<ModularController>().GetPlayerAnimator();
    }

    public abstract void AttackTrigger(AttackButton _button);
}
