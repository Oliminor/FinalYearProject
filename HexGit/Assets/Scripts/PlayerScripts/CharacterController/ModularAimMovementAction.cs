using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AimMovementAction", menuName = "ScriptableObjects/PlayerModular/AimMovementAction", order = 1)]
public class ModularAimMovementAction : ModularAction
{
    public override void ActionTrigger()
    {
        throw new System.NotImplementedException();
    }

    public override void ActionUpdate()
    {
        AimMovement();
    }

    private void AimMovement()
    {
        Vector3 eulerAngles = Camera.main.transform.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        rb.MoveRotation(Quaternion.Euler(eulerAngles));
    }
}
