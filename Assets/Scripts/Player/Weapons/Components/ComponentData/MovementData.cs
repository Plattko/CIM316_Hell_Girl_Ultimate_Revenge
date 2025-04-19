using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementData : ComponentData<AttackMovement>
{
    protected override void SetComponentDependency()
    {
        ComponentDependency = typeof(WeaponMovement);
    }
}
