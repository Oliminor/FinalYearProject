using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRangedWeapon : RangedWeapon
{
    public override bool AttackTrigger()
    {
        if (!player.IsAiming) return false;

        int currentMana = MainCharacterStats.instance.GetCurrentMana();                    

        int manaCost = GameManager.instance.DataManager.GetCost(Index);                         

        if (!GameManager.instance.CooldownManager.IsCooldownFinished(Index) || currentMana < manaCost) return false;                         

        GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(2, 0.1f);                          

        MainCharacterStats.instance.SetMana(manaCost);

        float cooldownTime = GameManager.instance.DataManager.GetCooldown(Index);

        GameManager.instance.CooldownManager.AddCooldown(Index, cooldownTime);

        anim.SetTrigger("arrowRelease");
        player.GetPlayerAnimator().SetTrigger("arrowRelease");

        float percent = GameManager.instance.DataManager.GetDamage(Index, 0) / 100.0f;             

        float damage = MainCharacterStats.instance.DamageCalculation() * percent; 

        GameObject go = Instantiate(projectilePrefab.gameObject, shootPos.position, transform.rotation);   
        go.AddComponent<PlayerBaseProjectile>();
        go.GetComponent<PlayerBaseProjectile>().SetProjectile(5000, 0.5f, damage, solidObject, hitEffectPrefab);

        return true;
    }
}
