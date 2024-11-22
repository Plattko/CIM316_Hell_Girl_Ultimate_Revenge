using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : ScriptableObject
{
    [HideInInspector] public SpellManager spellManager;
    
    [Header("Basic")]
    public int manaCost = 1;
    public float castCooldown = 0.1f;
    public float damage = 1f;

    public virtual void CastSpell()
    {

    }
}
