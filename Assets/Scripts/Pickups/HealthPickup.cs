using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private int healthPickupAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if the other collider is not the player
        if (!other.CompareTag("Player")) { return; }

        // Get a reference to the player's SpellManager script
        PlayerHealth playerHealth = other.GetComponentInChildren<PlayerHealth>();
        // Restore the player's mana if it's less than their max mana
        if (playerHealth.curHealth < playerHealth.maxHealth)
        {
            // Increase the player's mana by the mana count
            playerHealth.Heal(healthPickupAmount);
            // Destroy the mana pickup
            Destroy(gameObject);
        }
    }
}
