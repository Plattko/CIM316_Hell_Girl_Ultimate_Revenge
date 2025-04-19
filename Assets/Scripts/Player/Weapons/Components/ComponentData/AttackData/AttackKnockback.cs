using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AttackKnockback : AttackData
{
    [field: SerializeField] public float Strength { get; private set; }
}
