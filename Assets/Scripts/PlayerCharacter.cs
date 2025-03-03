using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerCharacter", menuName = "Character/Player")]
public class PlayerCharacter : ScriptableObject
{
    // Character attributes
    public string characterName;
    public int maxHealth;
    public Sprite fullHeartSprite;  // Full heart sprite
    public Sprite emptyHeartSprite; // Empty heart sprite
    public Sprite[] hearts;         // Array to hold current heart states

    private int currentHealth;

    // Initialize the player character with full health
    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        hearts = new Sprite[maxHealth];

        // Set all hearts to full at the start
        for (int i = 0; i < maxHealth; i++)
        {
            hearts[i] = fullHeartSprite;
        }
    }

    // Method to take damage, reducing health and updating the heart array
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();
    }

    // Method to heal and update hearts
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHearts();
    }

    // Update the heart sprites based on current health
    private void UpdateHearts()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < currentHealth)
            {
                hearts[i] = fullHeartSprite; // Full heart if health is still there
            }
            else
            {
                hearts[i] = emptyHeartSprite; // Empty heart if no health left
            }
        }
    }

    // Optional method to get the current heart sprites for UI purposes
    public Sprite[] GetHearts()
    {
        return hearts;
    }
}