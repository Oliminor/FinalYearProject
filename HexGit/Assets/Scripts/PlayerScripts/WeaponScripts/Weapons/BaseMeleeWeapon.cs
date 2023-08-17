using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeleeWeapon : MeleeWeapon
{
    [SerializeField] private Transform LeftFistWeapon;
    [SerializeField] private Transform RightFistWeapon;

    [SerializeField] private List<ParticleSystem> particleEffects;

    public override void AttackEnd()
    {
        base.AttackEnd();

        foreach (var _effects in particleEffects) _effects.Stop();
    }

    public override bool AttackTrigger()
    {
        if (!base.AttackTrigger()) return false;

        player.SetPlayerFace();

        foreach (var _effects in particleEffects) _effects.Play();

        return true;
    }

    /// <summary>
    /// Instantiate OverlapSphere for the left hand
    /// </summary>
    public void TriggerAutoAttackLeft()
    {
        float percent = GameManager.instance.DataManager.GetDamage(Index, comboIndex) / 100.0f;

        float damage = MainCharacterStats.instance.DamageCalculation() * percent;

        GameManager.instance.DamageCheck.PlayerSingleDamage(LeftFistWeapon.position, 2.0f, (int)damage, hitEffectPrefab.gameObject, solidObject, BuffTrigger.BaseAttack, 0);
    }

    /// <summary>
    /// Instantiate OverlapSphere for the right hand
    /// </summary>
    public void TriggerAutoAttackRight()
    {
        float percent = GameManager.instance.DataManager.GetDamage(Index, comboIndex) / 100.0f;

        float damage = MainCharacterStats.instance.DamageCalculation() * percent;

        GameManager.instance.DamageCheck.PlayerSingleDamage(RightFistWeapon.position, 2.0f, (int)damage, hitEffectPrefab.gameObject.gameObject, solidObject, BuffTrigger.BaseAttack, 0);
    }
}
