using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModularAction : ScriptableObject
{
    protected ModularController mController;
    public ModularController ModularController { get { return mController; } set { mController = value; } }

    protected CapsuleCollider capsuleCol;
    protected Animator anim;
    protected Rigidbody rb;
    protected Transform transform;
    protected LayerMask whatIsGround;
    protected float playerHeight;

    public virtual void Initialize()
    {
        transform = mController.transform;
        capsuleCol = mController.GetCapsuleCol();
        anim = mController.GetPlayerAnimator();
        rb = mController.GetPlayerRigidbody();
        whatIsGround = mController.GetWhatIsGround();
        playerHeight = capsuleCol.height * transform.localScale.y;
    }

    public abstract void ActionTrigger();
    public abstract void ActionUpdate();

    protected float GetFacingAngle()
    {
        Vector3 movementVector = new Vector3(GetInputVector().x, 0, GetInputVector().y);
        float targetAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + Camera.main.gameObject.transform.eulerAngles.y;
        return targetAngle;
    }
    protected Vector3 GetCameraForwardVector()
    {
        return GameLogic.GameLogic.GetCameraForwardVector(GetInputVector(), Camera.main);
    }

    protected Vector3 GetSlopeDirection()
    {
        return GameLogic.GameLogic.GetSlopeDirection(capsuleCol, GetInputVector(), Camera.main, whatIsGround, transform);
    }

    protected float GetForwardAngle()
    {
        return GameLogic.GameLogic.GetForwardAngle(capsuleCol, whatIsGround, transform);
    }

    protected float GetSlopeAngle()
    {
        return GameLogic.GameLogic.GetSlopeAngle(capsuleCol, 0.2f, whatIsGround, transform);
    }

    protected Vector2 GetInputVector()
    {
        return GameManager.instance.InputManager.GetInputVector();
    }

    protected bool InputCheck()
    {
        return GameManager.instance.InputManager.InputCheck();
    }

    protected bool IsInAir(float _raySize)
    {
        return GameLogic.GameLogic.IsInAir(_raySize, capsuleCol, transform);
    }

    protected bool IsGrounded()
    {
        return GameLogic.GameLogic.IsGrounded(capsuleCol, whatIsGround, transform);
    }
}
