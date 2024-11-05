using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    public Spell curSpell;

    public void Initialise(PlayerCharacter _playerCharacter)
    {
        playerCharacter = _playerCharacter;
    }

    public void UseSpell()
    {
        // Do nothing if the player doesn't have a spell
        if (curSpell == null) { return; }

        // If the player has more mana than the spell's mana cost, cast the spell
        if (playerCharacter.curMana >= curSpell.manaCost)
        {
            playerCharacter.UseMana(curSpell.manaCost);

            curSpell.CastSpell();
        }
    }
}
