using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [SerializeField] protected Transform projectilePrefab;
    [SerializeField] protected Transform shootPos;

    protected override void Initialize()
    {
        base.Initialize();
    }

    public override bool AttackTrigger()
    {
        throw new System.NotImplementedException();
    }

    public virtual void AimMode(bool _bool)
    {
        GameManager.instance.HUDManager.SetCrossHair(_bool);

        if (_bool)
        {
            GameManager.instance.CameraManager.ActivateCamera(GameManager.instance.CameraManager.GetAimCamera());
        }
        else
        {
            GameManager.instance.CameraManager.ActivateCamera(GameManager.instance.CameraManager.GetPlayerCamera());
        }
    }
}
