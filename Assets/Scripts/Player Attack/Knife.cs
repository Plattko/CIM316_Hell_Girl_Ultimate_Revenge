using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private float damage = 10f;  // Damage dealt by the knife

    // Set the damage when the knife is spawned
    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    // When the knife's trigger hits another 3D collider (enemy)
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hit has the EnemyHealth component
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Apply damage to the enemy
            enemyHealth.TakeDamage(damage);

            // Destroy the knife after hitting the enemy
            Destroy(gameObject);
        }
    }
}


