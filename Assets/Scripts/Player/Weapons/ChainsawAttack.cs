using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawAttack : MonoBehaviour
{
    public float attackCooldown = 1f;           // Cooldown between swings (for the first two swings)
    public float continuousDamageInterval = 0.1f; // Time interval for continuous damage
    public float damage = 10f;                   // Damage per hit
    public float resetTime = 1.5f;               // Time to reset the swing count after no attacks
    private int swingCount = 0;                  // Counter for the number of swings
    private float lastAttackTime = 0f;           // Time of the last attack
    private float lastSwingTime = 0f;            // Time of the last swing
    private bool isHoldingAttack = false;        // To track if the attack button is being held down

    void Update()
    {
        // Handle input for the first two swings
        if (Input.GetMouseButtonDown(0))
        {
            if (swingCount < 2 && Time.time >= lastAttackTime + attackCooldown)
            {
                ExecuteSwing();
                lastAttackTime = Time.time;
                lastSwingTime = Time.time;
                swingCount++;
            }
            // On the third swing, start holding the button for continuous damage
            else if (swingCount == 2)
            {
                isHoldingAttack = true;
                lastSwingTime = Time.time; // Reset the last swing time
            }
        }

        // Handle continuous damage while holding the attack button
        if (isHoldingAttack)
        {
            if (Time.time >= lastSwingTime + continuousDamageInterval)
            {
                ApplyContinuousDamage();
                lastSwingTime = Time.time; // Update the last swing time to the current time
            }
        }

        // Reset swing count if no attack occurs after resetTime (1.5 seconds)
        if (swingCount > 0 && Time.time - lastSwingTime > resetTime)
        {
            ResetSwingCount();
        }

        // Reset the swing counter when the attack button is released
        if (Input.GetMouseButtonUp(0))
        {
            if (isHoldingAttack)
            {
                isHoldingAttack = false; // Stop continuous damage
                ResetSwingCount();
            }
        }
    }

    private void ExecuteSwing()
    {
        // Handle the swing logic (e.g., animation, sound, etc.)
        Debug.Log("Swing #" + (swingCount + 1) + " executed.");
        // You can add any additional logic here, such as playing an animation or sound.
    }

    private void ApplyContinuousDamage()
    {
        // Logic to apply damage to enemies
        Debug.Log("Continuous damage applied: " + damage);
        // Use a method similar to your previous knife attack logic to apply damage
        // You might want to check for enemies in range and apply damage to them
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f); // Adjust the radius as needed
        foreach (var hitCollider in hitColliders)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void ResetSwingCount()
    {
        swingCount = 0;
        Debug.Log("Swing count reset.");
    }
}

