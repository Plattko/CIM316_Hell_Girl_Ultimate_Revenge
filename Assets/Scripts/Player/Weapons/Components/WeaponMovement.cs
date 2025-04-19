using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : WeaponComponent<MovementData, AttackMovement>
{
    protected override void Start()
    {
        base.Start();

        eventHandler.OnStartMovement += HandleStartMovement;
        eventHandler.OnStopMovement += HandleStopMovement;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        eventHandler.OnStartMovement -= HandleStartMovement;
        eventHandler.OnStopMovement -= HandleStopMovement;
    }

    private void HandleStartMovement()
    {
        PlayerController.SetVelocity(curAttackData.Velocity, PlayerController.WeaponAimPivot.forward * curAttackData.Direction.x);
    }

    private void HandleStopMovement()
    {
        PlayerController.SetVelocityZero();
    }
}
