using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    public Spell curSpell;

    private float castCooldownEnd = 0f;

    // Temporary variables for testing purposes
    public event Action<int> onManaUpdated;
    private int maxMana = 5;
    private int curMana;

    // Spell effects
    public event Action<float> onMovementPrevented;

    private void Start()
    {
        curMana = maxMana;
        InitialiseSpell();
    }

    private void Update()
    {
        // Debug use mana hotkey
        if (Input.GetKeyDown(KeyCode.N))
        {
            UseMana(1);
        }
        // Debug gain mana hotkey
        if (Input.GetKeyDown(KeyCode.M))
        {
            GainMana(1);
        }
    }

    public void Initialise(PlayerCharacter _playerCharacter)
    {
        playerCharacter = _playerCharacter;
    }

    public void InitialiseSpell()
    {
        // Give the current spell a reference to the Spell Manager
        curSpell.spellManager = this;
    }

    public void UseSpell()
    {
        // Do nothing if the player doesn't have a spell
        if (curSpell == null) { return; }
        // Do nothing if casting the spell is on cooldown
        if (Time.time < castCooldownEnd) { return; }
        // Do nothing if the player doesn't have enough mana to cast the spell
        if (curMana < curSpell.manaCost) { return; }

        // Reduce the player's mana count by the spell's mana cost
        //playerCharacter.UseMana(curSpell.manaCost);
        curMana -= curSpell.manaCost;
        // Signal that the mana has been updated
        onManaUpdated?.Invoke(curMana);
        // Cast the spell
        curSpell.CastSpell();
        // Set a new cast cooldown
        castCooldownEnd = Time.time + curSpell.castCooldown;

        if (curSpell.preventsMovement)
        {
            onMovementPrevented?.Invoke(curSpell.preventMovementDuration);
        }
    }

    private void UseMana(int amount)
    {
        // Check if the current mana is greater than 0
        if (curMana > 0)
        {
            // Reduce the player's current mana by the input amount
            curMana -= amount;
            // Prevent the current mana from dropping below 0
            curMana = Mathf.Clamp(curMana, 0, maxMana);
            // Signal that the mana has been updated
            onManaUpdated?.Invoke(curMana);
        }
    }

    private void GainMana(int amount)
    {
        // Check if the current mana is less than the max mana
        if (curMana < maxMana)
        {
            // Increase the current mana by the input amount
            curMana += amount;
            // Prevent the current mana from increasing above the player's max mana
            curMana = Mathf.Clamp(curMana, 0, maxMana);
            // Signal that the mana has been updated
            onManaUpdated?.Invoke(curMana);
        }
    }
}
