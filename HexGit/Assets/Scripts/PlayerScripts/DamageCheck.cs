using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCheck : MonoBehaviour
{
    private ModularController player;

    private void Start()
    {
        player =  GameManager.instance.ModularController;
    }

    /// <summary>
    ///  OverlapSphere check if the player hits the target or not
    /// </summary>

    public void PlayerSingleDamage(Vector3 _position, float _radius, int _damage, GameObject _hitEffect, LayerMask _target, BuffTrigger _buffTrigger, float _delay)
    {
        StartCoroutine(PlayerSingleDamageIE(_position, _radius, _damage, _hitEffect, _target, _buffTrigger, _delay));
    }

    IEnumerator PlayerSingleDamageIE(Vector3 _position, float _radius, int _damage, GameObject _hitEffect, LayerMask _target, BuffTrigger _buffTrigger, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Collider[] hitColliders = Physics.OverlapSphere(_position, _radius, _target);

        foreach (var hit in hitColliders)
        {
            if (hit.transform.TryGetComponent(out IDamageTaken _takeDamage))
            {
                _damage = _takeDamage.DamageCalculation(_damage); // damage calculation

                bool isCritical = MainCharacterStats.instance.GetCritical(); // is critical damage?

                Vector3 popUpPos = hit.ClosestPointOnBounds(_position); // pop up text position

                GameManager.instance.HUDManager.PopUpTextManager.DamageTextInstiate(_damage, popUpPos, isCritical, false); // instantiate the pop up text

                if (_hitEffect)
                {
                    Vector3 hitEffectDirection = hit.ClosestPoint(_position) - player.GetPlayerDamagePoint();

                    GameObject go = Instantiate(_hitEffect, popUpPos, Quaternion.LookRotation(hitEffectDirection.normalized)); // instantiate if there is any effect
                    Destroy(go, 0.5f);
                }

                GameManager.instance.BuffManager.BaseGemBuffTrigger(GameManager.instance.StanceManager.GetActiveStanceIndex(), _buffTrigger); // Activate buffs if they are triggered by damage

                float distance = Vector3.Distance(popUpPos, player.transform.position);

                if (distance < 8) GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(4, 0.15f); // shake the camera if the damage point close enough

                MainCharacterStats.instance.LifeStealCalculation(_damage); // lifesteal calculation for player

                _takeDamage.DamageTaken((int)_damage); // and the damage itself
            }
        }
    }

    /// <summary>
    ///  OverlapSphere check if the enemy hits the player or not
    /// </summary>

    public void EnemySingleDamage(Vector3 _position, float _radius, int _damage, LayerMask _target)
    {
        Collider[] hitColliders = Physics.OverlapSphere(_position, _radius, _target);

        foreach (var hit in hitColliders)
        {
            if (hit.transform.TryGetComponent(out IDamageTaken _takeDamage))
            {
                _takeDamage.DamageTaken((int)_damage);
            }

        }
    }
}
