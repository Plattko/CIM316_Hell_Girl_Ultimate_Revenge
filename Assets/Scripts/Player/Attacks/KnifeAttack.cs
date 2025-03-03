using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    public GameObject knifePrefab;           // Prefab for the knife object (3D GameObject with a sprite renderer)
    public Transform knifeSpawnPoint;        // Spawn point for the knife in front of the player
    public float knifeSpeed = 10f;           // Speed at which the knife moves forward
    public float baseDamage = 10f;           // Base damage of the knife attack
    public float damageMultiplier = 1.5f;    // Damage multiplier for every 5th attack
    public float attackCooldown = 0.5f;      // Cooldown between knife attacks
    public float resetTime = 1.0f;           // Time to reset the swing count after no attacks
    private float lastAttackTime = 0f;       // Time of the last attack
    private float lastSwingTime = 0f;        // Time of the last swing
    private int swingCount = 0;              // Counter for the number of swings

    void Update()
    {
        // Check if the attack button is pressed and if cooldown has passed
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            SpawnKnife();
            lastAttackTime = Time.time;
            lastSwingTime = Time.time;

            swingCount++;

            // Reset swing count if no attack occurs after 1 second
            if (Time.time - lastSwingTime > resetTime)
            {
                ResetSwingCount();
            }
        }

        // Reset swing count if no attack occurs for resetTime (1 second)
        if (Time.time - lastSwingTime > resetTime && swingCount > 0)
        {
            ResetSwingCount();
        }
    }

    // Function to spawn the knife object and move it forward
    void SpawnKnife()
    {
        // Instantiate the knife at the player's knife spawn point
        GameObject knife = Instantiate(knifePrefab, knifeSpawnPoint.position, knifeSpawnPoint.rotation);

        // Get the Knife script from the spawned knife and apply the correct damage
        Knife knifeScript = knife.GetComponent<Knife>();
        if (knifeScript != null)
        {
            float appliedDamage = baseDamage;

            // Check if this is the 5th swing to apply the damage multiplier
            if (swingCount == 5)
            {
                appliedDamage *= damageMultiplier;
                ResetSwingCount();  // Reset the swing count after applying the multiplier
            }

            // Pass the calculated damage to the Knife script
            knifeScript.SetDamage(appliedDamage);

            // Log the swing number and damage applied
            Debug.Log("Swing #" + swingCount + " applied with damage: " + appliedDamage);
        }

        // Add forward velocity to the knife using Rigidbody for 3D physics
        Rigidbody rb = knife.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector3(knifeSpeed, 0, 0);  // Move the knife along the X-axis

        }

        // Destroy the knife after 1 second to prevent it from flying infinitely
        Destroy(knife, 1f);
    }

    void ResetSwingCount()
    {
        swingCount = 0;
        Debug.Log("Swing count reset");
    }
}
