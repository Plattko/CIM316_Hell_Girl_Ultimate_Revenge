using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    // Player mana variables
    public event Action<int> onManaUpdated;
    private int maxMana = 5;
    private int curMana;

    // Spell variables
    public Spell curSpell;
    public event Action<float, bool> onSpellCast;
    private float cooldownTime;

    private void Start()
    {
        // Set the player's mana to their max mana
        curMana = maxMana;
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

    public void CastSpell()
    {
        // Do nothing if the player can't cast a spell
        if (!CanCast()) { return; }

        // Reduce the player's mana count by the spell's mana cost
        UseMana(curSpell.manaCost);
        // Cast the spell
        curSpell.Cast(gameObject);
        // Signal that a spell has been cast
        onSpellCast?.Invoke(curSpell.castTime, curSpell.lockoutDuringCast);
        // Set the spell cooldown time
        cooldownTime = Time.time + curSpell.cooldownTime;
    }

    private bool CanCast()
    {
        // Return false if the player doesn't have a spell
        if (curSpell == null) { return false; }
        // Return false if the spell is on cooldown
        if (Time.time < cooldownTime) { return false; }
        // Return false if the player doesn't have enough mana to cast the spell
        if (curMana < curSpell.manaCost) { return false; }
        return true;
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
