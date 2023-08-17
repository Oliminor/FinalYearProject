using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BaseEnemy : MonoBehaviour, IDamageTaken
{
    [SerializeField] protected string unitName;

    [SerializeField] protected int health;

    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask enemyTarget;
    [SerializeField] protected LayerMask whatIsSolid;

    [SerializeField] protected Transform eyeTransform;
    [SerializeField] protected Transform damagePoint;

    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float runningSpeed;

    [SerializeField] protected float autoAttackCooldown;
    [SerializeField] protected float viewAngle;
    [SerializeField] protected float hearingDistance;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float stopDistance;
    [SerializeField] protected float retreatDistance;
    [SerializeField] protected Rig rig;

    protected float movementSpeed;

    protected ModularController player;
    protected Rigidbody rb;
    protected CapsuleCollider capsule;
    protected Animator anim;

    protected int currentPathPoint;
    protected List<Vector3> AstarList = new();

    protected float defaultDrag;
    protected float height;

    protected delegate void DeathTrigger();
    protected DeathTrigger deathDelegate;

    protected Vector3 starPosition;
    protected Vector3 patrolPosition;

    protected bool isAttackCoolDownFinished = true;
    protected bool specialAttackUsed;

    private float movementAnimLerp;
    private float idleAnimLerp;
    private int idleIndex;
    private int idleAnimCurrentLoop;

    public string GetUnitName() { return unitName; }

    protected virtual void StateMachine() { }

    protected void Initialize()
    {
        player =  GameManager.instance.ModularController;
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        starPosition = transform.position;
        defaultDrag = rb.drag;
        height = capsule.height * transform.localScale.y;

        SetUpRigAimTarget();

        deathDelegate += TriggerBuffTrigger;
    }

    /// <summary>
    /// Interface to deal the damage when the player attacks
    /// </summary>
    virtual public void DamageTaken(int _damageNumber)
    {
        health -= _damageNumber;

        if (health <= 0)
        {
            deathDelegate();
        }
    }

    /// <summary>
    /// Calculates the damage (if the enemy has shield or something, this will reduces the damage)
    /// </summary>
    public int DamageCalculation(int _damageReturn)
    {
        return _damageReturn;
    }

    // Destroys the gameobject
    protected void DestroyThisUnit()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// If the enemy has animation rigging rig, this will setups the target
    /// </summary>
    private void SetUpRigAimTarget()
    {
        if (!rig) return;

        for (int i = 0; i < rig.transform.childCount; i++)
        {
            var sourceObjects = rig.transform.GetChild(0).GetComponent<MultiAimConstraint>().data.sourceObjects;

            sourceObjects.SetTransform(0, player.GetComponent<ModularController>().GetPlayerDamageTransform());

            rig.transform.GetChild(0).GetComponent<MultiAimConstraint>().data.sourceObjects = sourceObjects;
        }

        GetComponent<RigBuilder>().Build();
    }

    protected void AnimationManager()
    {
        int movementIndex = 0;

        if (movementSpeed <= runningSpeed) movementIndex = 2;
        if (movementSpeed <= walkSpeed) movementIndex = 1;
        if (movementSpeed == 0) movementIndex = 0;

        movementAnimLerp = Mathf.Lerp(movementAnimLerp, movementIndex, 0.1f);

        anim.SetFloat("speed", movementAnimLerp);

        if ((int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime > idleAnimCurrentLoop)
        {
            idleAnimCurrentLoop = (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            idleIndex = Random.Range(0, 2);
        }

        idleAnimLerp = Mathf.Lerp(idleAnimLerp, idleIndex, 0.1f);

        anim.SetFloat("idle", idleAnimLerp);
    }

    /// <summary>
    /// Triggers the auto attack
    /// </summary>
    virtual protected void TriggerAttack() { }

    /// <summary>
    /// Cooldown between the auto attacks
    /// </summary>
    protected void TriggerAttackCooldown()
    {
        StartCoroutine(AttackCooldown());
    }

    /// <summary>
    /// Triggers the special attack (ultimate)
    /// </summary>
    protected virtual void TriggerSpecialAttack() { }

    /// <summary>
    /// Triggers the special attack effect 
    /// </summary>
    protected virtual void TriggerSpecialAttackEffect() { }

    // Cooldown timer
    IEnumerator AttackCooldown()
    {
        isAttackCoolDownFinished = false;
        yield return new WaitForSeconds(autoAttackCooldown);
        isAttackCoolDownFinished = true;
    }

    /// <summary>
    /// Stationarey state so the player enemy doesn't move
    /// </summary>
    virtual protected void Stationary()
    {
        rb.velocity = Vector3.zero;

        movementSpeed = 0;
    }

    /// <summary>
    /// When the enemy falling the drag is set to 0
    /// </summary>
    virtual protected void Falling()
    {
        rb.drag = 0;
    }

    /// <summary>
    /// Returns true if the enemy reaches the current path target
    /// </summary>
    protected bool IsTargetReached(Vector3 _target)
    {
        Vector2 patroPosV2 = new Vector2(_target.x, _target.z);
        Vector2 tranPosV2 = new Vector2(transform.position.x, transform.position.z);
        float distance = Vector2.Distance(patroPosV2, tranPosV2);

        if (distance > 1) return false;

        return true;
    }

    /// <summary>
    ///  Returns true if the enemy left his own zone
    /// </summary>
    protected bool IsEnemyLeftArea()
    {
        float distance = Vector3.Distance(starPosition, transform.position);

        if (distance < retreatDistance) return false;

        return true;
    }

    /// <summary>
    /// Smooth turning for more believeable movement
    /// </summary>
    protected void SmoothTurning(Vector3 _targetPosition, float turnSpeed)
    {
        Quaternion lookRotation = Quaternion.LookRotation((_targetPosition - transform.position).normalized);
        Vector3 angle = lookRotation.eulerAngles;
        angle = new Vector3(0, angle.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angle), turnSpeed);
    }

    /// <summary>
    /// Faces towards the player
    /// </summary>
    protected void FaceToPlayer()
    {
        Quaternion lookRotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);
        Vector3 angle = lookRotation.eulerAngles;
        angle = new Vector3(0, angle.y, 0);
        transform.rotation = Quaternion.Euler(angle);
    }

    /// <summary>
    /// Returns true if the enemy is able to reach the player (based on attack distance + retreat distance)
    /// </summary>
    protected bool isPlayerReachable()
    {
        return IsPlayerInRange(attackDistance + retreatDistance - 3);
    }

    /// <summary>
    /// returns true if the player in attack range
    /// </summary>
    protected bool isPlayerInAttackRange()
    {
        return IsPlayerInRange(attackDistance);
    }

    /// <summary>
    /// returns true if the player is hearing distance
    /// </summary>
    protected bool IsPlayerHeard()
    {
        return IsPlayerInRange(hearingDistance);
    }

    /// <summary>
    /// returns if the player is in stop distance (too close)
    /// </summary>
    protected bool IsTooCloseToThePlayer()
    {
        return IsPlayerInRange(stopDistance);
    }

    /// <summary>
    /// Triggers this if the enemy dies (certain buffs are activates when the enemy dies)
    /// </summary>
    private void TriggerBuffTrigger()
    {
        GameManager.instance.BuffManager.BaseGemBuffTrigger(GameManager.instance.StanceManager.GetActiveStanceIndex(), BuffTrigger.EnemyEliminiate);
    }

    /// <summary>
    /// Returns true if the enemy is in a certain distance from the player
    /// </summary>
    protected bool IsPlayerInRange(float _distance)
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance > _distance) return false;

        return true;
    }

    /// <summary>
    /// returns true if the player is on the view (cone check)
    /// </summary>
    protected bool IsPlayerOnView()
    {
        //bool isPlayerBehindSomething = true;
        Vector3 raycastPoint = eyeTransform.position;
        float raySize = viewDistance;

        Vector3 direction = player.GetPlayerDamagePoint() - eyeTransform.position;

        if (Physics.Raycast(raycastPoint, direction, out RaycastHit hitinfo, raySize))
        {
            if (hitinfo.transform == player.transform) return true;
        }

        //float distance = Vector3.Distance(player.position, transform.position);

        //if (isPlayerBehindSomething && GetViewAngle() < viewAngle) return false;

        //if (distance > viewDistance) return false;

        return false;
    }

    /// <summary>
    ///  returns the angle between the player and the enemy for cone chekcing
    /// </summary>
    protected float GetViewAngle()
    {
        Vector3 forward = eyeTransform.forward;
        Vector3 toPlayer = player.transform.position - eyeTransform.position;

        float angle = Vector3.Dot(forward.normalized, toPlayer.normalized);
        float degree = Mathf.Acos(angle) * Mathf.Rad2Deg;

        return degree;
    }

    /// <summary>
    /// returns true if the enemy is on the ground
    /// </summary>
    protected bool IsOnTheGround()
    {
        Vector3 raycastPoint = capsule.bounds.center;
        float height = capsule.height * transform.localScale.x;
        float raySize = height * 0.5f + 0.5f;

        if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, whatIsGround))
        {
            return true;
        }

        return false;
    }
}
