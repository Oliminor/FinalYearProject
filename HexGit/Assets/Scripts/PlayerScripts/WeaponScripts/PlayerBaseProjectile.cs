using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private float activeGravityTime;
    private float damage;
    private Transform hitEffect;
    private LayerMask target;

    private Vector3 previousPosition;

    public void SetProjectile(float _force, float _activeGravityTime, float _damage, LayerMask _target, Transform _hitEffect)
    {
        
        previousPosition = transform.position;
        activeGravityTime = _activeGravityTime;
        rb = gameObject.AddComponent<Rigidbody>();
        damage = _damage;
        target = _target;
        hitEffect = _hitEffect;


        rb.useGravity = false;
        rb.AddForce(transform.forward * _force, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileMovement();
    }

    private void ProjectileMovement()
    {
        activeGravityTime -= Time.deltaTime;

        if (activeGravityTime <= 0) rb.useGravity = true;

        float distance = Vector3.Distance(previousPosition, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(previousPosition, transform.forward, out hit, distance, target))
        {
            GameManager.instance.DamageCheck.PlayerSingleDamage(hit.point, 0.3f, (int)damage, null, target, BuffTrigger.BaseAttack, 0);

            GameObject groundHitParticle = Instantiate(hitEffect.gameObject, hit.point, Quaternion.identity);
            Destroy(groundHitParticle, 0.5f);
            Destroy(gameObject);
        }

        previousPosition = transform.position;
    }
}
