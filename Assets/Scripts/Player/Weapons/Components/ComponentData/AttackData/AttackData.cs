using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
    [SerializeField, HideInInspector] private string name;

    public void SetAttackName(int attackNum) => name = $"Attack {attackNum}";
}
