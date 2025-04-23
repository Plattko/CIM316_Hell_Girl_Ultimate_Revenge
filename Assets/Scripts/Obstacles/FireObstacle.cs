using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObstacle : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        Destroy(gameObject);
    }
}
