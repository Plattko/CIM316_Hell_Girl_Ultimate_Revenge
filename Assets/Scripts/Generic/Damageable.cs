using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Damageable : MonoBehaviour
{
    public event Action<float> onDamaged;

    public void Damage(float amount)
    {
        onDamaged?.Invoke(amount);
    }
}
