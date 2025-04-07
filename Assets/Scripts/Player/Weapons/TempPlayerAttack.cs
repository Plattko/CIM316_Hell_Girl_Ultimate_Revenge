using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerAttack : MonoBehaviour
{
    int damage = 5;

    private void OnTriggerEnter(Collider other)
    {
        // Deal damage if it hit a damageable object
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Deal damage
            damageable.TakeDamage(damage);
            // Print the damage dealt
            Debug.Log("Damaged " + other.name + " for " + damage + " damage.");
        }
    }
}
