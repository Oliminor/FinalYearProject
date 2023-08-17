using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "MovementAction", menuName = "ScriptableObjects/PlayerModular/MovementAction", order = 1)]
public class ModularMovementAction : ModularAction
{
    [SerializeField] private float slopeLimit;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jogSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float movementLerpSpeed;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float edgeDetectionRaySize;

    private float movementSpeed;
    private float lerpMovementSpeed;
    private float smoothTurn;

    private bool isWalking;
    private bool isMovementLerping;

    private Vector3 lerpPosition;

    public override void ActionTrigger() { }

    public override void Initialize()
    {
        base.Initialize();
        GameManager.instance.InputManager.GetInputActions().Player.Walk.performed += WalkingToggle;
    }

    private void WalkingToggle(InputAction.CallbackContext context) => isWalking = !isWalking;

    public override void ActionUpdate()
    {
        MovementSpeed();
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector3 moveDirection = GetCameraForwardVector();

        if (GetSlopeAngle() < slopeLimit && GetForwardAngle() < slopeLimit) moveDirection = GetSlopeDirection();

        lerpMovementSpeed = Mathf.Lerp(lerpMovementSpeed, movementSpeed, movementLerpSpeed);

        mController.GetPlayerRigidbody().velocity = moveDirection.normalized * lerpMovementSpeed;

        SpeedLimit();
        SmoothTurning();
        //EdgeDetection();
    }

    private void EdgeDetection()
    {
        if (!InputCheck()) return;

        float edgeAngle;
        Vector3 position;

        Vector3 raycastPoint = capsuleCol.bounds.center + (transform.forward * capsuleCol.radius * transform.localScale.y * edgeDetectionRaySize);
        float raySize = playerHeight * 0.5f;

        Debug.DrawLine(raycastPoint, raycastPoint - new Vector3(0, raySize, 0), Color.red);

        if (Physics.Raycast(raycastPoint, Vector3.down, out RaycastHit hitinfo, raySize, whatIsGround))
        {
            edgeAngle = Vector3.Angle(transform.up, hitinfo.normal);
            position = hitinfo.point;
        }

        else return;

        if (edgeAngle > slopeLimit) return;

        isMovementLerping = true;
        lerpPosition = position;
        mController.StartCoroutine(LerpSafety());
    }
    IEnumerator LerpSafety()
    {
        yield return new WaitForSeconds(0.3f);
        if (isMovementLerping) isMovementLerping = !isMovementLerping;
    }

    private void MovementSpeed()
    {
        movementSpeed = jogSpeed;
        if (GameManager.instance.InputManager.GetInputActions().Player.Sprint.inProgress && MainCharacterStats.instance.GetCurrentStamina() > 0)
        {
            MainCharacterStats.instance.SetStamina(0.5f);
            isWalking = false;
            movementSpeed = sprintSpeed;
        }
        if (isWalking) movementSpeed = walkSpeed;
        if (!InputCheck()) movementSpeed = 0;
    }

    private void SpeedLimit()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (velocity.magnitude > movementSpeed)
        {
            Vector3 limit = velocity.normalized * movementSpeed;
            rb.velocity = new Vector3(limit.x, rb.velocity.y, limit.z);
        }
    }

    private void SmoothTurning()
    {
        float targetAngle = GetFacingAngle();

        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurn, turnSmoothTime);

        if (InputCheck()) transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
