using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;  // Maximum health of the enemy
    private float currentHealth;   // Current health of the enemy

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the enemy's health
        currentHealth = maxHealth;
    }

    // Function to take damage from an attack
    public void TakeDamage(float damageAmount)
    {
        // Reduce the enemy's health by the damage amount
        currentHealth -= damageAmount;

        // Check if the enemy's health reaches 0 or below
        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log("Enemy took damage: " + damageAmount + ", Current health: " + currentHealth);
    }

    // Function to handle the enemy's death
    void Die()
    {
        Debug.Log("Enemy has died!");

        // Destroy the enemy object (or trigger death animation)
        Destroy(gameObject);
    }
}
