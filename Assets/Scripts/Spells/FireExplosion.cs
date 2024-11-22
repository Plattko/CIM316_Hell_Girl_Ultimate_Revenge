using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : MonoBehaviour
{
    [HideInInspector] public float damage;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the enemy is damageable
        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null)
        {
            // Damage the enemy
            damageable.Damage(damage);
            Debug.Log("Damaged enemy for " + damage + " damage.");
        }
    }
}
