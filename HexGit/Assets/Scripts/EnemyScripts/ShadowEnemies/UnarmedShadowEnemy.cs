using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnarmedShadowEnemy : LandEnemy
{
    [SerializeField] private float attackMovementSpeed;

    [SerializeField] private int punchAttackDamage;
    [SerializeField] private float punchAttackRadius;
    [SerializeField] private int specialAttackDamage;
    [SerializeField] private float specialAttackRadius;

    void Start()
    {
        Initialize();
        PatrolTimeTrigger();
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
        AnimationManager();
        TriggerSpecialAttack();
    }

    /// <summary>
    /// Triggers the special attack 
    /// </summary>
    protected override void TriggerSpecialAttack()
    {
        if (specialAttackUsed) return;

        if (health < 1000 && health > 0)
        {
            rb.useGravity = false;
            capsule.enabled = false;
            specialAttackUsed = true;
            anim.SetTrigger("specialAttack");
            SetEnemyStatus(EnemyStatus.SPECIALATTACK);
        }
    }

    /// <summary>
    /// Triggers the special attack effect using animator
    /// </summary>
    protected override void TriggerSpecialAttackEffect()
    {
        GameObject damageSphere = new GameObject("SpecialAoeDamageFromEnemy");
        damageSphere.transform.position = transform.position;
        damageSphere.AddComponent<AoeDamage>().SetStats(1, specialAttackDamage, specialAttackRadius, 0, enemyTarget, true, true);

        capsule.enabled = true;
        rb.useGravity = true;
        Transform go = Instantiate(specialAttackEffect, transform.position, Quaternion.identity);
        Destroy(go.gameObject, 5);
        if (IsPlayerInRange(30)) GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(2, 0.1f);
    }

    /// <summary>
    /// Triggers the death effect using the animator
    /// </summary>
    protected void DeathEffect()
    {
        gameObject.GetComponent<SkinnedMeshEffect>().SetIsEnemyDead(); ;
        capsule.enabled = false;
    }

    /// <summary>
    /// Instantiates the attack sphere check to damage the player
    /// </summary>
    protected void AttacPunchSphereTrigger()
    {
        damagePoint.GetComponent<AoeDamage>().SetStats(0, punchAttackDamage, punchAttackRadius, 0, enemyTarget, false, true);
    }
}
