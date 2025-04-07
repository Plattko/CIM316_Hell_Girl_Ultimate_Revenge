using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : WeaponComponent
{
    private MovementData data;

    protected override void Awake()
    {
        base.Awake();

        data = weapon.Data.GetData<MovementData>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        eventHandler.OnStartMovement += HandleStartMovement;
        eventHandler.OnStopMovement += HandleStopMovement;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        eventHandler.OnStartMovement -= HandleStartMovement;
        eventHandler.OnStopMovement -= HandleStopMovement;
    }

    private void HandleStartMovement()
    {
        var curAttackData = data.AttackData[weapon.CurAttackCounter];

        Debug.Log("Start movement.");
        PlayerController.SetVelocity(curAttackData.Velocity, curAttackData.Direction);
    }

    private void HandleStopMovement()
    {
        Debug.Log("Stop movement.");
        PlayerController.SetVelocityZero();
    }
}
