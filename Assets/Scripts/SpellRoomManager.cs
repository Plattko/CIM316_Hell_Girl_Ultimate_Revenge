using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRoomManager : MonoBehaviour
{
    [SerializeField] private List<Spell> spells = new List<Spell>();
    [SerializeField] private SpellPedestal[] spellPedestals;

    private void OnEnable()
    {
        // Connect every spell pedestals's onSpellChosen event to the SpellChosen function
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            spellPedestal.onSpellChosen += SpellChosen;
        }
    }

    private void OnDisable()
    {
        // Disconnect every spell pedestals's onSpellChosen event from the SpellChosen function
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            spellPedestal.onSpellChosen -= SpellChosen;
        }
    }

    private void Start()
    {
        // Create a list of the available spells
        List<Spell> availableSpells = spells;
        // Give each pedestal a spell from the list of available spells without repetition
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            // Roll a random spell from the available spells list
            int roll = Random.Range(0, availableSpells.Count);
            // Initialise the spell item with the rolled spell
            spellPedestal.Initialise(availableSpells[roll]);
            // Remove the spell from the available spells list
            availableSpells.RemoveAt(roll);
        }
    }

    public void SpellChosen()
    {
        // Destroy each pedestal's spell sprite
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            // If the pedestal's spell sprite exists, destroy it
            if (spellPedestal.spellSprite != null)
            {
                Destroy(spellPedestal.spellSprite);
            }
            // Destroy the pedestal's script so it can't be interacted with
            Destroy(spellPedestal);
        }
    }
}
