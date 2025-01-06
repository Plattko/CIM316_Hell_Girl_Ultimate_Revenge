using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    // Player mana variables
    private int maxMana = 5;
    private int curMana;
    public event Action<int> onManaUpdated;

    // Spell variables
    [SerializeField] private PlayerAim aimPivot;
    public Spell curSpell;
    private float cooldownTime;
    public event Action<float, bool> onSpellCast;

    [SerializeField] private GameObject spellItemPrefab;
    public event Action<Spell> onSpellUpdated;

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

    //-------------------------------------------------------------
    // CASTING
    //-------------------------------------------------------------
    public void CastSpell()
    {
        // Do nothing if the player can't cast a spell
        if (!CanCast()) { return; }

        // Reduce the player's mana count by the spell's mana cost
        UseMana(curSpell.manaCost);
        // Cast the spell
        if (curSpell.hasDirection)
        {
            curSpell.CastWithDirection(gameObject, aimPivot.transform.forward);
        }
        else
        {
            curSpell.Cast(gameObject);
        }
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

    //-------------------------------------------------------------
    // SPELL MANAGEMENT
    //-------------------------------------------------------------
    public void SwapSpell(Spell newSpell)
    {
        // If the player already has a spell, drop it
        if (curSpell != null)
        {
            DropSpell();
        }
        // Set the player's current spell to the new spell
        curSpell = newSpell;
        // Signal that the spell has been updated
        onSpellUpdated?.Invoke(newSpell);
    }

    private void DropSpell()
    {
        // Set the drop position to slightly in front of the spell manager
        Vector3 dropPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
        // Instantiate the spell item prefab
        SpellItem spellItem = Instantiate(spellItemPrefab, dropPos, Quaternion.identity).GetComponent<SpellItem>();
        // Set the spell item's spell to the player's current spell
        spellItem.Initialise(curSpell);
        // Clear the player's current spell
        curSpell = null;
    }

    //-------------------------------------------------------------
    // MANA MANAGEMENT
    //-------------------------------------------------------------
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
