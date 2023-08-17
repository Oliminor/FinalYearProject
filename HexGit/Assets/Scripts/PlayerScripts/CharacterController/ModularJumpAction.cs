using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAction", menuName = "ScriptableObjects/PlayerModular/JumpAction", order = 1)]
public class ModularJumpAction : ModularAction
{
    [SerializeField] private float jumpForce;

    public override void ActionTrigger()
    {
        float jumpPositioShift = capsuleCol.radius * transform.localScale.y * 2f;
        transform.position = new Vector3(transform.position.x, transform.position.y + jumpPositioShift, transform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(0, GetFacingAngle(), 0));
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        anim.SetTrigger("jump");
    }

    public override void ActionUpdate() { }
}
