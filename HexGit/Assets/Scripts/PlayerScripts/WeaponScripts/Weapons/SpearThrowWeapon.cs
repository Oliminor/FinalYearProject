using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearThrowWeapon : RangedWeapon
{
    public override bool AttackTrigger()
    {
        int currentMana = MainCharacterStats.instance.GetCurrentMana();

        int manaCost = GameManager.instance.DataManager.GetCost(Index);

        if (!GameManager.instance.CooldownManager.IsCooldownFinished(Index) || currentMana < manaCost) return false;

        MainCharacterStats.instance.SetMana(manaCost);

        DissolveEffect(true);

        float cooldownTime = GameManager.instance.DataManager.GetCooldown(Index);

        GameManager.instance.CooldownManager.AddCooldown(Index, cooldownTime);

        player.GetPlayerAnimator().CrossFade(skillModule.AttackModuleList[0].AnimationName, 0.1f);
        anim.speed = animationSpeed;
        anim.CrossFade(skillModule.AttackModuleList[0].AnimationName, 0.1f);

        player.IsAttacking = true;

        return true;
    }

    private void InstantiateSpear()
    {
        DissolveAmountUpdate(1);

        GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(5, 0.2f);

        Instantiate(projectilePrefab, shootPos.position, shootPos.rotation);

        player.IsAttackFalse();
    }
}
