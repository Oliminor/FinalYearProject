using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    protected int comboIndex;
    protected int attackIndex;

    private IEnumerator comboIndexReset;

    public int AttackIndex { get { return attackIndex; } }

    public virtual void AttackEnd()
    {
        DissolveEffect(false);
        player.IsAttackFalse();
        ResetAttackIndexTrigger();
        anim.CrossFade("Idle", 1.0f);
    }

    private void ResetAttackIndexTrigger()
    {
        comboIndexReset = ResetAttackIndex();
        StartCoroutine(comboIndexReset);
    }

    private void StopResetAttackIndex()
    {
        if (comboIndexReset == null) return;
        StopCoroutine(comboIndexReset);
        comboIndexReset = null;
    }

    IEnumerator ResetAttackIndex()
    {
        yield return new WaitForSeconds(0.5f);
        attackIndex = 0;
        comboIndexReset = null;
    }

    public override bool AttackTrigger()
    {
        if (MainCharacterStats.instance.GetCurrentStamina() <= 0) return false;

        if (!GameManager.instance.CooldownManager.IsCooldownFinished(Index)) return false;

        float cooldownTime = GameManager.instance.DataManager.GetCooldown(Index);

        GameManager.instance.CooldownManager.AddCooldown(Index, cooldownTime);

        StopResetAttackIndex();

        DissolveEffect(true);
        player.IsAttacking = true;

        player.GetPlayerAnimator().speed = animationSpeed;
        player.GetPlayerAnimator().CrossFade(skillModule.AttackModuleList[attackIndex].AnimationName, 0.1f);
        anim.speed = animationSpeed;
        anim.CrossFade(skillModule.AttackModuleList[attackIndex].AnimationName, 0.1f);

        comboIndex = skillModule.AttackModuleList[attackIndex].ComboIndex;

        attackIndex++;
        if (attackIndex == skillModule.AttackModuleList.Count) attackIndex = 0;

        MainCharacterStats.instance.SetStamina(GameManager.instance.DataManager.GetCost(skillModule.AttackModuleList[attackIndex].Index));

        return true;
    }
}
