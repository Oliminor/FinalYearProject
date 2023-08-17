using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected int index;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask solidLayer;
    [SerializeField] protected GameObject hitEffect;
    [SerializeField] protected float speed;

    protected Rigidbody rb;
    protected Vector3 previousPosition;
    protected int damage;

    virtual protected void Initialize()
    {
        rb = GetComponent<Rigidbody>();

        float percent = GameManager.instance.DataManager.GetDamage(index, 0) / 100.0f;

        damage = (int)(MainCharacterStats.instance.DamageCalculation() * percent);

        previousPosition = transform.position;
    }
}
