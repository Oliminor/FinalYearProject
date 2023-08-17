using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlam : MeleeWeapon
{
    [SerializeField] private Transform damagePoint;

    [SerializeField] private List<ParticleSystem> particleEffects;

    [SerializeField] private GameObject slamEffect;

    public override void AttackEnd()
    {
        base.AttackEnd();

        player.GetPlayerAnimator().applyRootMotion = false;

        foreach (var _effects in particleEffects) _effects.Stop();
    }

    public override bool AttackTrigger()
    {
        if (!base.AttackTrigger()) return false;

        player.SetPlayerFace();

        player.GetPlayerAnimator().applyRootMotion = true;

        foreach (var _effects in particleEffects) _effects.Play();

        return true;
    }

    /// <summary>
    /// Instantiate OverlapSphere for the left hand
    /// </summary>
    public void TriggerSlamAttack()
    {
        GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(6, 0.3f);

        float percent = GameManager.instance.DataManager.GetDamage(Index, comboIndex) / 100.0f;

        float damage = MainCharacterStats.instance.DamageCalculation() * percent;

        GameManager.instance.DamageCheck.PlayerSingleDamage(damagePoint.position, 8.0f, (int)damage, hitEffectPrefab.gameObject, solidObject, BuffTrigger.SkillAttack, 0);

        if (slamEffect)
        {
            GameObject go = Instantiate(slamEffect, damagePoint.position, Quaternion.identity);
            Destroy(go, 2f);
        }
    }
}
