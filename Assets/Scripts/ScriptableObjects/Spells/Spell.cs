using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Item
{
    [Header("Spell Variables")]
    public int manaCost = 1;
    public float damage = 1f;

    public float castTime;
    public float cooldownTime;
    public bool movementLockoutDuringCast = false;

    public bool hasDirection = false;

    public virtual void Cast(GameObject parent) { }
    public virtual void CastWithDirection(GameObject parent, Vector3 dir) { }
}
