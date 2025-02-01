using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    public float attackCooldown = 0.5f;     // Time between swings
    public float resetTime = 1.0f;          // Time to reset the swing count after no attacks
    public float damage = 10f;              // Base damage of the attack
    public float damageMultiplier = 1.5f;   // Damage multiplier on the 5th swing
    private int swingCount = 0;             // Counter for the number of swings
    private float lastAttackTime = 0f;      // Time when the last attack occurred
    private float lastSwingTime = 0f;       // Time when the last swing occurred

    // Update is called once per frame
    void Update()
    {
        // Check if the attack button is pressed (example: left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            PerformAttack();
        }

        // Reset swing count if 1 second has passed without an attack
        if (Time.time - lastSwingTime > resetTime && swingCount > 0)
        {
            ResetSwingCount();
        }
    }

    void PerformAttack()
    {
        // Check if the cooldown has passed since the last attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            swingCount++;
            lastAttackTime = Time.time;
            lastSwingTime = Time.time;

            // Check for the 5th swing to apply the damage multiplier
            float appliedDamage = damage;
            if (swingCount == 5)
            {
                appliedDamage *= damageMultiplier;
                ResetSwingCount(); // Reset the swing count after the 5th swing
            }

            // Call a function to apply damage (to the enemy, etc.)
            ApplyDamage(appliedDamage);

            Debug.Log("Swing #" + swingCount + " applied with damage: " + appliedDamage);
        }
    }

    void ResetSwingCount()
    {
        swingCount = 0;
        Debug.Log("Swing count reset");
    }

    void ApplyDamage(float damageAmount)
    {
        // Perform a raycast or check for enemy in range (simple example using raycast)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))  // Adjust range as needed
        {
            // Check if the object hit has the EnemyHealth component
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Apply damage to the enemy
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }
}
