using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEnemy : BaseEnemy
{
    public enum EnemyStatus { IDLE, PATROL, CHASE, ATTACK, SPECIALATTACK, DAZED, DEATH, FALLING }

    public EnemyStatus enemyStatus;

    [SerializeField] protected PatrolPath path;
    [SerializeField] protected float patrollingAreaSize;
    [SerializeField] protected Vector2 guardTimeMinMax;
    [SerializeField] protected Transform specialAttackEffect;

    IEnumerator patrolIE;

    protected void SetEnemyStatus(EnemyStatus _enemyStatus) {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("specialAttack")) return;
        enemyStatus = _enemyStatus; }

    /// <summary>
    /// Basic state machine for the LandEnemy
    /// </summary>
    protected override void StateMachine()
    {
        switch (enemyStatus)
        {
            case EnemyStatus.IDLE: 
                IdleStatus();
                break;
            case EnemyStatus.PATROL:
                PatrolStatus();
                break;
            case EnemyStatus.CHASE:
                ChaseStatus();
                break;
            case EnemyStatus.ATTACK:
                AttackStatus();
                break;
            case EnemyStatus.SPECIALATTACK:
                SpecialAttackStatus();
                break;
            case EnemyStatus.DAZED:
                DazedStatus();
                break;
            case EnemyStatus.DEATH:
                DeathStatus();
                break;
            case EnemyStatus.FALLING:
                FallingStatus();
                break;
        }

        if (Vector3.Distance(starPosition, new Vector3(starPosition.x, transform.position.y, starPosition.z)) > 50) Destroy(gameObject);
    }

    protected new void Initialize()
    {
        base.Initialize();

        if (patrollingAreaSize != 0)
        {
            patrolPosition = NewPatrolPoint();
            SetEnemyStatus(EnemyStatus.PATROL);
        }
        if (path) if (path.GetPathList().Count == 0) path = null;
    }

    /// <summary>
    /// Additional functions to the base Damate Taken
    /// </summary>
    public override void DamageTaken(int _damageNumber)
    {
        base.DamageTaken(_damageNumber);

        if (health <= 0 && enemyStatus != EnemyStatus.DEATH)
        {
            SetEnemyStatus(EnemyStatus.DEATH);
            anim.SetTrigger("death");
            return;
        }

        if (enemyStatus == EnemyStatus.SPECIALATTACK) return;

        if (enemyStatus == EnemyStatus.PATROL || enemyStatus == EnemyStatus.IDLE)
        {
            if (patrollingAreaSize == 0) return;

            SetEnemyStatus(EnemyStatus.CHASE);
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("autoAttacking"))
        {
            FaceToPlayer();
            SetEnemyStatus(EnemyStatus.DAZED);
            anim.SetTrigger("hitReceived");
        }
    }

    // State Functions
    #region States

    // Idle state
    protected virtual void IdleStatus()
    {
        ToChase();                                  // Condition to change to Chase state
        Stationary();                               // Not moving state
    }
    // Patrol state
    protected virtual void PatrolStatus()
    {
        EdgeDetection();                            // Edge detection to climb up somewhere if needed
        TriggerFalling();                           // Condition to change to falling state
        ToChase();                                  // Condition to change to Chase state
        SmoothTurning(patrolPosition, 0.05f);
        Movement(walkSpeed);                        // Movement to the next patrol point

        // If patrol point reached, new patrol point asked
        if (IsTargetReached(patrolPosition))
        {
            if (AstarList.Count > 1 || currentPathPoint != 0)
            {
                if (patrollingAreaSize == 0) return;
                patrolPosition = NewPatrolPoint();
                return;
            }
            SetEnemyStatus(EnemyStatus.IDLE);
            PatrolTimeTrigger();
        }
    }

    protected virtual void ChaseStatus()
    {
        if (patrollingAreaSize == 0) SetEnemyStatus(EnemyStatus.IDLE);

        TriggerFalling();                           // Condition to change to falling state
        EdgeDetection();                            // Edge detection to climb up somewhere if needed
        ToAttack();                                 // Condition to change to attack state
        Movement(runningSpeed);                     // Movement towards the player

        if (IsPlayerInRange(10))
        {
            SmoothTurning(player.transform.position, 0.1f);
        }
        else
        {
            if (patrolPosition != transform.position)
            {
                SmoothTurning(patrolPosition, 0.1f);
            }
        }

        ResetPositionIfLeftArea();                  // Retreat if outside from it's own area
        NewAstarPath(player.GetPlayerDamagePoint());

        if (IsTargetReached(patrolPosition))
        {
            if (AstarList.Count > 1)
            {
                patrolPosition = NextAstarPosition();
            }
        }
    }

    // Attack state
    protected virtual void AttackStatus()
    {
        if (!isPlayerInAttackRange() && !anim.GetCurrentAnimatorStateInfo(0).IsTag("autoAttacking")) SetEnemyStatus(EnemyStatus.CHASE); // If the player too far away, continue chasing



        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("autoAttacking") && isAttackCoolDownFinished)
        {
            capsule.enabled = true; // safety

            anim.SetTrigger("normalAttack");

            TriggerAttack();

            TriggerAttackCooldown();
        }

        SmoothTurning(player.transform.position, 0.05f);

        Stationary();                               // Not moving state
    }

    // Special attack state
    protected virtual void SpecialAttackStatus()
    {
        Stationary();                               // Not moving state

        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("specialAttack")) SetEnemyStatus(EnemyStatus.CHASE);
    }

    // Dazed state
    protected virtual void DazedStatus()
    {
        Stationary();                               // Not moving state
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("dazed")) SetEnemyStatus(EnemyStatus.CHASE);
    }
    // Death state
    protected virtual void DeathStatus()
    {
        Stationary();                           // Not moving state
    }
    // Fallinf state
    protected virtual void FallingStatus()
    {
        Falling();

        if (IsOnTheGround())
        {
            NewAstarPath(starPosition);
            PatrolTimeTrigger();
            SetEnemyStatus(EnemyStatus.IDLE);
            rb.drag = defaultDrag;
        }
    }

    #endregion
    /// <summary>
    /// Triggers falling state when the enemy has no ground under the leg
    /// </summary>
    protected void TriggerFalling()
    {
        if (!IsOnTheGround()) SetEnemyStatus(EnemyStatus.FALLING);
    }

    /// <summary>
    /// Triggers Chase state when the player is on heard or view distance
    /// </summary>
    protected void ToChase()
    {
        if (!isPlayerReachable()) return;
        if (IsPlayerHeard() || IsPlayerOnView()) SetEnemyStatus(EnemyStatus.CHASE);
    }

    /// <summary>
    /// Triggers attack state when the player is on attack range
    /// </summary>
    void ToAttack()
    {
        if (isPlayerInAttackRange())
        {
            SetEnemyStatus(EnemyStatus.ATTACK);
        }
    }

    /// <summary>
    /// Triggers patrol m ode when the enemy is too far away from it's own zone
    /// </summary>
    void ResetPositionIfLeftArea()
    {
        if (IsEnemyLeftArea() || !IsPlayerOnView())
        {
            SetEnemyStatus(EnemyStatus.PATROL);
            NewAstarPath(starPosition);
        }
    }

    /// <summary>
    /// Triggers the cooldown between 2 patrol points (patrol -> guard -> patrol)
    /// </summary>
    protected void PatrolTimeTrigger()
    {
        if (patrolIE == null)
        {
            patrolIE = PatrolTimer(GetRandomGuardTime());
            StartCoroutine(patrolIE);
        }
        else return;
    }

    // same but the Coroutine
    protected IEnumerator PatrolTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        if (enemyStatus != EnemyStatus.IDLE)
        {
            patrolIE = null;
            yield break;
        }
        SetEnemyStatus(EnemyStatus.PATROL);
        patrolPosition = NewPatrolPoint();
        patrolIE = null;
    }

    /// <summary>
    /// Movement with a custom speed
    /// </summary>
    virtual protected void Movement(float _speed)
    {
        Vector3 moveDirection = GetSlopeDirection();

        movementSpeed = _speed;

        rb.velocity = moveDirection.normalized * _speed;
    }

    /// <summary>
    /// Get random guard time between the set min and max guard time
    /// </summary>
    protected float GetRandomGuardTime()
    {
        return Random.Range(guardTimeMinMax.x, guardTimeMinMax.y);
    }

    /// <summary>
    /// Get the new patrol position
    /// </summary>
    protected Vector3 NewPatrolPoint()
    {
        Vector3 patrolPoint = starPosition;

        // New patrol position from the pathfinder list
        if (AstarList.Count > 1)
        {
            return NextAstarPosition();
        }

        // New patrol position from the patrol path (custom loop)
        if (path)
        {
            if (currentPathPoint >= path.GetPathList().Count) currentPathPoint = 0;
            patrolPoint = path.GetPathList()[currentPathPoint];
            currentPathPoint++;
            return patrolPoint;
        }

        if (patrollingAreaSize == 0) return starPosition;

        // Astar calculation for the next patrol position
        if (AstarList.Count < 2)
        {
            float y = starPosition.y + 2;
            float x = starPosition.x + Random.Range(-patrollingAreaSize, patrollingAreaSize);
            float z = starPosition.z + Random.Range(-patrollingAreaSize, patrollingAreaSize);

            patrolPoint = new Vector3(x, y, z);

            NewAstarPath(patrolPoint);
        }

        if (AstarList.Count > 0) return AstarList[0];

        return patrolPoint;
    }

    public Vector3 NextAstarPosition()
    {
        AstarList.RemoveAt(0);
        Vector3 patrolPoint = AstarList[0];
        return patrolPoint;
    }

    public void NewAstarPath(Vector3 _patrolPoint)
    {
        AstarList.Clear();

        AstarList.AddRange(GameManager.instance.PathFinder.AStar(transform.position + Vector3.up * 2, _patrolPoint));
    }

    /// <summary>
    /// Slope direction calculation, so the enemy able to walks straight to the direction (otherwise the slopes could change the course or the movement speed)
    /// </summary>
    private Vector3 GetSlopeDirection()
    {
        Vector3 direction = transform.forward;
        Vector3 raycastPoint = capsule.bounds.center;
        float height = capsule.height * transform.localScale.x;
        float raySize = height * 0.5f + 0.3f;

        if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, whatIsGround))
        {
            Vector3 forward = transform.forward;
            Vector3 up = new Vector3(0, 1.0f, 0);
            Vector3 right = Vector3.Cross(up.normalized, forward.normalized);
            direction = Vector3.Cross(right, hitinfo.normal).normalized;

            Debug.DrawRay(raycastPoint, direction);
        }

        return direction;
    }

    /// <summary>
    /// When the enemy reaches edges (other objects) it will hop on it if can)
    /// </summary>
    private void EdgeDetection()
    {
        float edgeAngle;
        Vector3 position = transform.position;

        Vector3 raycastPoint = capsule.bounds.center + (transform.forward * capsule.radius * transform.localScale.y * 1);
        float raySize = height * 0.5f;

        Debug.DrawLine(raycastPoint, raycastPoint - new Vector3(0, raySize, 0), Color.red);

        if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, whatIsGround))
        {
            position = hitinfo.point;
            if (GetGameobjectUnderCharacter() == hitinfo.transform.gameObject) return;
        }

        if (!Physics.Raycast(raycastPoint, Vector3.down, raySize * 2f))
        {
            SetEnemyStatus(EnemyStatus.IDLE);
            PatrolTimeTrigger();
        }

        transform.position = position;
    }

    /// <summary>
    /// Returns the gameobject under the enemy for the edge detection (only able to hop on different object than the current (awful solution))
    /// </summary>
    private GameObject GetGameobjectUnderCharacter()
    {
        GameObject _gameObject = this.gameObject;
        Vector3 raycastPoint = capsule.bounds.center;
        float raySize = height * 0.5f + 0.3f;

        if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, whatIsGround))
        {
            _gameObject = hitinfo.transform.gameObject;
        }

        return _gameObject;
    }
}
