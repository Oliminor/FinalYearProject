using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeDamage : MonoBehaviour
{
    bool isDinamicallyPlaced;
    bool singleDamage;
    private float lifeTime;
    private int damage;
    private float radius;
    private float dotRate;
    private LayerMask target;

    public void SetStats(float _lifeTime, int _damage, float _radius, float _dotrate, LayerMask _target, bool _isDinamicallyPlaced, bool _singleDamage)
    {
        isDinamicallyPlaced = _isDinamicallyPlaced;
        singleDamage = _singleDamage;
        lifeTime = _lifeTime;
        damage = _damage;
        radius = _radius;
        dotRate = _dotrate;
        target = _target;

        Initialize();
    }

    private void Initialize()
    {
        if (singleDamage) SingleDamage();
        else StartCoroutine(DotDamage());
        if (isDinamicallyPlaced) Destroy(gameObject, lifeTime);
    }

    // Do damage overtime
    IEnumerator DotDamage()
    {
        while(true)
        {
            yield return new WaitForSeconds(dotRate);

            SingleDamage();
        }
    }

    /// <summary>
    /// Collider check (if the player inside, deals damage)
    /// </summary>
    private void SingleDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, target);

        foreach (var hit in hitColliders)
        {
            if (hit.transform.TryGetComponent(out IDamageTaken _takeDamage))
            {
                _takeDamage.DamageTaken(damage);
            }
        }
    }

}
