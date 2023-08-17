using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class EnemyProjectile : MonoBehaviour
{
    protected float speed;
    protected float radius;
    protected int damage;
    protected LayerMask target;

    protected bool isLanded = false;
    protected bool isArrow = false;

    protected Transform hitEffect;
    protected Transform landingEffect;
    protected Vector3 previousPosition;

    protected bool isDamageIndicatorOn = false;

    protected bool isHomingMissle = false;
    protected float homingMissleTurnSpeed;

    protected ModularController player;

    public void SetArrowValues(bool _isArrow, float _speed, int _damage, float _radius, LayerMask _layer, Transform _hitEffect, Transform _landingEffect, float _destroyTime)
    {
        isArrow = _isArrow;

        speed = _speed;
        damage = _damage;
        target = _layer;
        radius = _radius;

        if (_hitEffect) hitEffect = _hitEffect;
        if (_landingEffect) landingEffect = _landingEffect;

        Destroy(gameObject, _destroyTime);

        Initailaze();
    }

    protected virtual void Update()
    {
        HomingMissle();

        ProjectileMovement();
        CollideCheck();
    }

    protected virtual void Initailaze()
    {
        previousPosition = transform.position;
        player =  GameManager.instance.ModularController;
    }

    /// <summary>
    /// Curves towards the target while moving
    /// </summary>
    protected virtual void HomingMissle()
    {
        if (!isHomingMissle) return;

        Vector3 toTarget = (player.GetPlayerDamagePoint() - transform.position).normalized;

        if (Vector3.Dot(toTarget, transform.forward) < 0) return;

        Quaternion lookRotation = Quaternion.LookRotation((player.GetPlayerDamagePoint() - transform.position).normalized);
        Vector3 angle = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angle), homingMissleTurnSpeed);
    }

    /// <summary>
    /// Checks if the impact target (if there any, ground, or player)
    /// </summary>
    protected virtual void CollideCheck()
    {
        if (isLanded) return;

        float distance = Vector3.Distance(previousPosition, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(previousPosition, transform.forward, out hit, distance, target))
        {
            if (hit.transform.TryGetComponent(out IDamageTaken _takeDamage))
            {
                _takeDamage.DamageTaken(damage);

                if (player.GetIsPlayerInvulnarable()) return;

                ProjectileHitPlayer(hit);
            }
            else
            {
                ProjectileLanded(hit);
            }

            isLanded = true;
            DestroyThis();
        }
        previousPosition = transform.position;
    }

    /// <summary>
    /// Forward movement (simple projectile)
    /// </summary>
    protected virtual void ProjectileMovement()
    {
        if (isLanded) return;

        transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
    }

    // Destroy call
    protected virtual void DestroyThis()
    {
        Destroy(gameObject, 2);
    }

    /// <summary>
    /// When the projectile hits the player (sticks inside the player for a while)
    /// </summary>
    protected virtual void ProjectileHitPlayer(RaycastHit hit)
    {
        GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(3, 0.1f);

        if (hitEffect)
        {
            GameObject go = Instantiate(hitEffect.gameObject, hit.point, transform.rotation);
            Destroy(go, 3);
            Destroy(gameObject, 3);
        }

        if (isArrow)
        {
            transform.SetParent( GameManager.instance.ModularController.GetPlayerDamageTransform());
            float stickingOutPos = Random.Range(1.5f, 3.0f);
            transform.position = transform.position - transform.forward * stickingOutPos;
        }
    }

    /// <summary>
    /// When the projectile lands on on the solid object (Sticks there too)
    /// </summary>
    protected virtual void ProjectileLanded(RaycastHit hit)
    {
        if (isArrow)
        {
            float stickingOutPos = Random.Range(1.5f, 3.0f);
            transform.position = transform.position - transform.forward * stickingOutPos;
        }

        if (landingEffect)
        {
            if (radius > 0) GameManager.instance.DamageCheck.EnemySingleDamage(hit.point, radius, damage, target);
            GameObject go = Instantiate(landingEffect.gameObject, hit.point, transform.rotation);
            Destroy(go, 3);
            Destroy(gameObject, 3);
        }

        if (isDamageIndicatorOn)
        {
            transform.GetChild(2).GetComponent<DecalProjector>().fadeFactor = 0;
        }
    }

    /// <summary>
    /// Add damage indicator which is a projectile to show the impact point
    /// </summary>
    public void AddDamageIndicator()
    {
        GameObject go = GameManager.instance.DamageIndicatorManager.AddDamageIndicator(transform, new Vector3(0, 0, 0), new Vector2(radius * 2, radius * 2), 500);
        go.GetComponent<DamageIndicatorLerp>().TriggerDamageIndicator();
        isDamageIndicatorOn = true;
    }

    // Turns on the homing missle projectile state
    public void TurnOnHomingMissle(float _turnSpeed)
    {
        isHomingMissle = true;
        homingMissleTurnSpeed = _turnSpeed;
    }
}
