using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShadowEnemy : LandEnemy
{
    [SerializeField] private Transform bowWeapon;
    [SerializeField] private Transform arrowProjectile;
    [SerializeField] private Transform fakeArrowProjectile;
    [SerializeField] private Transform handEffect;
    [SerializeField] private Transform landEffect;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private int arrowDamage;

    [SerializeField] private float specialAttackTime;
    [SerializeField] private int specialAttackDamage;
    [SerializeField] private float specialAttackRadius;
    [SerializeField] private float specialAttackDotRate;

    private BowString bowString;

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

    private new void Initialize()
    {
        base.Initialize();

        handEffect.GetComponent<ParticleSystem>().Stop();
        anim = GetComponent<Animator>();
        bowString = bowWeapon.GetComponent<BowString>();
        InstantiateArrowInHand();
        if (fakeArrowProjectile.TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(0);
    }

    /// <summary>
    /// Instantiate a fake projectile to the hand (the release arrow projectile is different)
    /// </summary>
    private void InstantiateArrowInHand()
    {
        fakeArrowProjectile = Instantiate(fakeArrowProjectile, bowString.GetRightHand().position, Quaternion.identity, bowString.GetRightHand());
        fakeArrowProjectile.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        fakeArrowProjectile.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Instantiate the real projectile
    /// </summary>
    private void InstantiateProjectile()
    {
        GameObject go = Instantiate(arrowProjectile.gameObject, fakeArrowProjectile.position, fakeArrowProjectile.rotation);
        if (go.TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(1);
        go.transform.LookAt(player.GetComponent<ModularController>().GetPlayerDamagePoint());
        go.GetComponent<ShadowArrowProjectile>().SetArrowValues(true, arrowSpeed, arrowDamage, 0, whatIsSolid, null, landEffect, 15);
    }

    /// <summary>
    /// Draw mode on: The bow string is attached to the enemy hand
    /// </summary>
    public void BowDrawModeOn()
    {
        rig.weight = 1;
        bowString.SetDrawMode(true);
        if (fakeArrowProjectile.TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveOn();
    }

    /// <summary>
    /// Draw mode off: The bow string released 
    /// </summary>
    public void BowDrawModeOff()
    {
        rig.weight = 0;
        bowString.SetDrawMode(false);
        if (fakeArrowProjectile.TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(0);
        InstantiateProjectile();
    }

    /// <summary>
    /// Starts the special attack
    /// </summary>
    protected override void TriggerSpecialAttack()
    {
        if (specialAttackUsed) return;

        if (IsPlayerInRange(5))
        {
            anim.SetTrigger("specialAttack");
            SetEnemyStatus(EnemyStatus.SPECIALATTACK);
            specialAttackUsed = true;
        }
    }

    /// <summary>
    /// Triggers the special effect for the special attack
    /// </summary>
    protected override void TriggerSpecialAttackEffect()
    {
        GameObject go = Instantiate(specialAttackEffect.gameObject, transform.position + transform.up * 1, Quaternion.identity);
        go.AddComponent<AoeDamage>().SetStats(specialAttackTime, specialAttackDamage, specialAttackRadius, specialAttackDotRate, enemyTarget, true, false);
    }

    /// <summary>
    /// Triggers the death effect using the animator
    /// </summary
    protected void DeathEffect()
    {
        gameObject.GetComponent<SkinnedMeshEffect>().SetIsEnemyDead();

        if (bowWeapon.TryGetComponent(out DissolveEffectManager _dissolveWeapon)) _dissolveWeapon.DissolveOff();

        if (fakeArrowProjectile.TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveOff();

        capsule.enabled = false;
    }
}
