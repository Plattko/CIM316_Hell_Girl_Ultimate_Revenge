using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellRoom : MonoBehaviour
{
    [SerializeField] private List<Spell> availableSpells = new List<Spell>();
    [SerializeField] private SpellPedestal[] spellPedestals;

    private void OnEnable()
    {
        // Connect every spell pedestals's onSpellChosen event to the SpellChosen function
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            spellPedestal.onSpellChosen += OnSpellChosen;
        }
    }

    private void OnDisable()
    {
        // Disconnect every spell pedestals's onSpellChosen event from the SpellChosen function
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            spellPedestal.onSpellChosen -= OnSpellChosen;
        }
    }

    private void Start()
    {
        // Create a list of the available spells
        List<Spell> unselectedSpells = availableSpells;
        // Give each pedestal a spell from the list of available spells without repetition
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            // Roll a random spell from the available spells list
            int roll = Random.Range(0, unselectedSpells.Count);
            // Initialise the spell item with the rolled spell
            spellPedestal.Initialise(unselectedSpells[roll]);
            // Remove the spell from the available spells list
            unselectedSpells.RemoveAt(roll);
        }
    }

    private void OnSpellChosen()
    {
        // Destroy each pedestal's spell sprite
        foreach (SpellPedestal spellPedestal in spellPedestals)
        {
            // Unsubscribe from the pedestal's spell chosen event
            spellPedestal.onSpellChosen -= OnSpellChosen;
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
