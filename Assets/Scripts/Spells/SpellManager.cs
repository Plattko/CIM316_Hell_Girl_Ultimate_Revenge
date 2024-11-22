using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    public Spell curSpell;

    private float castCooldownEnd = 0f;

    // Temporary variables for testing purposes
    private int maxMana = 5;
    private int curMana;

    public void Start()
    {
        curMana = maxMana;
        InitialiseSpell();
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

        // If the player has more mana than the spell's mana cost, cast the spell
        if (curMana >= curSpell.manaCost)
        {
            // Reduce the player's mana count by the spell's mana cost
            //playerCharacter.UseMana(curSpell.manaCost);
            curMana -= curSpell.manaCost;
            // Cast the spell
            curSpell.CastSpell();
            // Set a new cast cooldown
            castCooldownEnd = Time.time + curSpell.castCooldown;
        }
    }
}
