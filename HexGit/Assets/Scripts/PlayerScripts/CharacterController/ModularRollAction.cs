using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RollAction", menuName = "ScriptableObjects/PlayerModular/RollAction", order = 1)]
public class ModularRollAction : ModularAction
{
    [SerializeField] private float rollSpeed;    
    [SerializeField] private float rollStaminaAmount;
    [SerializeField] private float invurnalableTime;

    public override void ActionTrigger()
    {
        MainCharacterStats.instance.SetStamina(rollStaminaAmount);

        rb.transform.rotation = Quaternion.Euler(new Vector3(0, GetFacingAngle(), 0));

        anim.speed = 1;
        anim.SetTrigger("Roll");
        mController.TriggerInvurnalableTimer(invurnalableTime);
    }

    public override void ActionUpdate()
    {
        RollMovement();
    }

    private void RollMovement()
    {
        rb.velocity = transform.forward * rollSpeed;
    }


}
