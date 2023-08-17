using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowSentryWeapon : RangedWeapon
{
    [SerializeField] private LayerMask sentryTarget;
    [SerializeField] private float sentryRadious;

    public override bool AttackTrigger()
    {
        int currentMana = MainCharacterStats.instance.GetCurrentMana();

        int manaCost = GameManager.instance.DataManager.GetCost(Index);

        if (!GameManager.instance.CooldownManager.IsCooldownFinished(Index) || currentMana < manaCost) return false;

        MainCharacterStats.instance.SetMana(manaCost);

        float cooldownTime = GameManager.instance.DataManager.GetCooldown(Index);

        GameManager.instance.CooldownManager.AddCooldown(Index, cooldownTime);

        DissolveEffect(true);

        float percent = GameManager.instance.DataManager.GetDamage(Index, 0) / 100.0f;

        float damage = MainCharacterStats.instance.DamageCalculation() * percent;

        StartCoroutine(SentryUpdate((int)damage));

        return true;
    }

    IEnumerator SentryUpdate(int _damage)
    {
        float duration = GameManager.instance.DataManager.GetDuration(Index);

        while (duration >= 0)
        {
            yield return new WaitForSeconds(1);
            duration -= 1;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, sentryRadious, sentryTarget);

            if (hitColliders.Length == 0) continue;

            float smallestDistance = Mathf.Infinity;
            Quaternion target = Quaternion.identity;

            foreach (var hit in hitColliders)
            {
                if (hit.transform.TryGetComponent(out IDamageTaken _takeDamage))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);

                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        target = Quaternion.LookRotation(hit.transform.position - transform.position);
                    }
                }
            }

            transform.rotation = target;

            GameObject go = Instantiate(projectilePrefab.gameObject, shootPos.position, transform.rotation);
            go.AddComponent<PlayerBaseProjectile>();
            go.GetComponent<PlayerBaseProjectile>().SetProjectile(5000, 0.5f, _damage, solidObject, hitEffectPrefab);
        }

        DissolveEffect(false);
    }
}
