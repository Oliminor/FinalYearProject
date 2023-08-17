using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum playerStatus { GROUND, AIR }
public enum GroundAction { MOVEMENT, AIM, ROLL, ATTACKING, IDLE }

public class ModularController : MonoBehaviour, IDamageTaken
{
    public playerStatus status;
    public GroundAction groundAction;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform damagePos;
    [SerializeField] private float drag;
    [SerializeField] private float isInAirRaySize;
    [SerializeField] private float jumpDownModifier;
    [SerializeField] private float jumpUpModifier;

    [SerializeField] ModularAction movementAction;
    [SerializeField] ModularAction rollAction;
    [SerializeField] ModularAction jumpAction;
    [SerializeField] ModularAction AimMovementAction;

    private StanceManager stanceManager;

    private CapsuleCollider capsuleCol;
    private Animator anim;
    private Rigidbody rb;

    private float fallingDistance;
    private Vector3 positionBeforeAirTime;

    private bool isAiming;
    private bool isJumping;
    private bool isRolling;
    private bool isInvulnarable;
    private bool isAttacking;

    public bool IsAiming { get { return isAiming; } set { isAiming = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public Animator GetPlayerAnimator() { return anim; }
    public Rigidbody GetPlayerRigidbody() { return rb; }
    public CapsuleCollider GetCapsuleCol() { return capsuleCol; }
    public LayerMask GetWhatIsGround() { return whatIsGround; }
    public playerStatus GetCharacterStatus() { return status; }
    public void SetPlayerStatus(playerStatus _status) { status = _status; }
    public void SetCharacterGroundAction(GroundAction _groundActions) { groundAction = _groundActions; }
    public Vector3 GetPlayerDamagePoint() { return damagePos.position; }
    public Transform GetPlayerDamageTransform() { return damagePos; }
    public bool GetIsPlayerInvulnarable() { return isInvulnarable; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        capsuleCol = GetComponent<CapsuleCollider>();
        anim.applyRootMotion = false;
    }

    private void Start()
    {
        stanceManager = GameManager.instance.StanceManager;

        movementAction.ModularController = this;
        rollAction.ModularController = this;
        jumpAction.ModularController = this;
        AimMovementAction.ModularController = this;

        movementAction.Initialize();
        rollAction.Initialize();
        jumpAction.Initialize();
        AimMovementAction.Initialize();

        InputDeleagate();
    }

    void FixedUpdate()
    {
        RootSelector();

        switch (status)
        {
            case playerStatus.GROUND:

                GroundSelector();
                ResetFallingCheck();

                switch (groundAction)
                {
                    case GroundAction.MOVEMENT:
                        movementAction.ActionUpdate();
                        break;
                    case GroundAction.AIM:
                        AimMovementAction.ActionUpdate();
                        break;
                    case GroundAction.ROLL:
                        rollAction.ActionUpdate();
                        break;
                    case GroundAction.ATTACKING:
                        rb.velocity = Vector3.zero;
                        break;
                    case GroundAction.IDLE:
                        rb.velocity = Vector3.zero;
                        break;
                }

                GravityModifier();

                break;
            case playerStatus.AIR:
                AirMovement();
                FallingCheck();
                break;
        }

        AnimationManager();
        GravityLimit();
        DragManager();
    }

    private void InputDeleagate()
    {
        GameManager.instance.InputManager.GetInputActions().Player.Jump.performed += Jump;
        GameManager.instance.InputManager.GetInputActions().Player.Roll.performed += Roll;
        GameManager.instance.InputManager.GetInputActions().Player.LeftClickAttack.performed += LeftClickAttack;
        GameManager.instance.InputManager.GetInputActions().Player.ESkillAttack.performed += ESkillAttack;
        GameManager.instance.InputManager.GetInputActions().Player.QSkillAttack.performed += QSkillAttack;
    }

    public bool isBusy()
    {
        if (!IsGrounded()) return true;

        if (isAttacking) return true;

        if (isRolling) return true;

        return false;
    }

    private void LeftClickAttack(InputAction.CallbackContext context)
    {
        if (isBusy()) return;

        stanceManager.AttackTrigger(AttackButton.LEFTCLICK);
    }

    private void ESkillAttack(InputAction.CallbackContext context)
    {
        if (isBusy()) return;

        stanceManager.AttackTrigger(AttackButton.SKILL02);
    }

    private void QSkillAttack(InputAction.CallbackContext context)
    {
        if (isBusy()) return;

        stanceManager.AttackTrigger(AttackButton.SKILL01);
    }

    public void IsAttackFalse() 
    {
        anim.speed = 1;
        isAttacking = false;
        if (!InputCheck()) anim.CrossFade("Idle", 0.15f);
        else anim.CrossFade("RunningBlendTree", 0.15f);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isBusy()) return;

        if (IsAiming) return;

        if (MainCharacterStats.instance.GetCurrentStamina() <= 0) return;

        isJumping = true;

        StartCoroutine(GroundCheckDelay());

        jumpAction.ActionTrigger();
    }

    private void Roll(InputAction.CallbackContext context)
    {
        if (isBusy()) return;

        if (IsAiming) return;

        if (MainCharacterStats.instance.GetCurrentStamina() <= 0) return;

        isRolling = true;

        rollAction.ActionTrigger();
    }

    public void RollFinished() => isRolling = false;

    private void RootSelector()
    {
        if (isJumping || IsInAir(isInAirRaySize)) SetPlayerStatus(playerStatus.AIR);

        if (IsGrounded()) SetPlayerStatus(playerStatus.GROUND);
    }

    private void GroundSelector()
    {
        if (isAttacking) SetCharacterGroundAction(GroundAction.ATTACKING);

        else if (isRolling) SetCharacterGroundAction(GroundAction.ROLL);

        else if (isAiming) SetCharacterGroundAction(GroundAction.AIM);

        else if (InputCheck())
        {
            SetCharacterGroundAction(GroundAction.MOVEMENT);
            movementAction.ActionTrigger();
        }

        else if (!InputCheck()) SetCharacterGroundAction(GroundAction.IDLE);
    }

    IEnumerator GroundCheckDelay()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
    }

    private void FallingCheck()
    {
        fallingDistance = transform.position.y - positionBeforeAirTime.y;
        if (fallingDistance < -50)
        {
            MainCharacterStats.onGameOver();
            positionBeforeAirTime = transform.position;
        }
    }

    private void ResetFallingCheck()
    {
        fallingDistance = 0;
        positionBeforeAirTime = transform.position;
    }

    private void AirMovement()
    {
        rb.useGravity = true;

        if (rb.velocity.y < 0) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * jumpDownModifier, rb.velocity.z);
        if (rb.velocity.y > 0) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / jumpUpModifier, rb.velocity.z);
    }

    public void TriggerInvurnalableTimer(float _time) { StartCoroutine(TriggerInvurnalableTimerIE(_time)); }

    private IEnumerator TriggerInvurnalableTimerIE(float _time)
    {
        isInvulnarable = true;
        yield return new WaitForSeconds(_time);
        isInvulnarable = false;
    }

    private void GravityModifier()
    {
        if (IsGrounded()) rb.useGravity = false;
        else rb.useGravity = true;

        if (!IsGrounded()) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - 10, rb.velocity.z);
    }

    private void AnimationManager()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        anim.SetFloat("magnitude", GetInputVector().magnitude);
        anim.SetFloat("speed", flatVel.magnitude);
        anim.SetBool("isGrounded", status == playerStatus.GROUND);
        anim.SetBool("isAiming", isAiming);


        switch (status)
        {
            case playerStatus.GROUND:
                anim.SetBool("isInAir", IsInAir(isInAirRaySize));
                break;

            case playerStatus.AIR:
                anim.SetBool("isInAir", IsInAir(1));
                break;
        }
    }

    public void SetPlayerFace()
    {
        rb.transform.rotation = Quaternion.Euler(0f, GetFacingAngle(), 0f);
    }

    public float GetFacingAngle()
    {
        Vector3 movementVector = new Vector3(GetInputVector().x, 0, GetInputVector().y);
        float targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + Camera.main.gameObject.transform.eulerAngles.y;
        return targetAngle;
    }

    public void FacePlayerTo(Vector3 _lookAt)
    {
        Quaternion lookRotation = Quaternion.LookRotation((_lookAt - transform.position).normalized);
        Vector3 angle = lookRotation.eulerAngles;
        angle = new Vector3(0, angle.y, 0);
        transform.rotation = Quaternion.Euler(angle);
    }

    private void DragManager()
    {
        if (rb.useGravity == true) rb.drag = 0;
        else rb.drag = drag;
    }

    private void GravityLimit()
    {
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -20, 20), rb.velocity.z);
    }

    private bool InputCheck()
    {
        return GameManager.instance.InputManager.InputCheck();
    }

    private bool IsInAir(float _raySize)
    {
        return GameLogic.GameLogic.IsInAir(_raySize, capsuleCol, transform);
    }

    private bool IsGrounded()
    {
        return GameLogic.GameLogic.IsGrounded(capsuleCol, whatIsGround, transform);
    }
    private Vector2 GetInputVector()
    {
        return GameManager.instance.InputManager.GetInputVector();
    }

    public void DamageTaken(int _damageNumber)
    {
        if (isInvulnarable) return;
        MainCharacterStats.instance.TakeDamage(_damageNumber);
    }

    public int DamageCalculation(int _damageReturn)
    {
        return 0;
    }
}
