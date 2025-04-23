using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackData : ComponentData<AttackKnockback>
{
    protected override void SetComponentDependency()
    {
        ComponentDependency = typeof(WeaponKnockback);
    }
}
