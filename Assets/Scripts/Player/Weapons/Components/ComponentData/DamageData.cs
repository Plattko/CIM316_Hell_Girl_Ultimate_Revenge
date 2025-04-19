using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData : ComponentData<AttackDamage>
{
    protected override void SetComponentDependency()
    {
        ComponentDependency = typeof(WeaponDamage);
    }
}
