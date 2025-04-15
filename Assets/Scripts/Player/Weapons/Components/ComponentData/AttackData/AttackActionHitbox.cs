using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AttackActionHitbox : AttackData
{
    public bool Debug;
    
    [field: SerializeField] public Vector3 HitboxOffset { get; private set; }
    [field: SerializeField] public float HitboxRadius { get; private set; }
}
