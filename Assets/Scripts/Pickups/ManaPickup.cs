using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPickup : MonoBehaviour
{
    private int manaPickupAmount = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        // Do nothing if the other collider is not the player
        if (!other.CompareTag("Player")) { return; }

        // Get a reference to the player's SpellManager script
        SpellManager spellManager = other.GetComponentInChildren<SpellManager>();
        // Restore the player's mana if it's less than their max mana
        if (spellManager.curMana < spellManager.maxMana)
        {
            // Increase the player's mana by the mana count
            spellManager.GainMana(manaPickupAmount);
            // Destroy the mana pickup
            Destroy(gameObject);
        }
    }
}
