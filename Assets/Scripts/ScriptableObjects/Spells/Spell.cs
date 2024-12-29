using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject
{
    [Header("Common Variables")]
    public new string name;
    public int manaCost = 1;
    public float damage = 1f;

    public float castTime;
    public bool lockoutDuringCast = false;
    public float cooldownTime;

    public virtual void Cast(GameObject parent) { }
}
