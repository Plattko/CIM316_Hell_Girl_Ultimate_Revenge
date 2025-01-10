using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItem : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Spell spell;

    public void Initialise(Spell _spell)
    {
        spell = _spell;
        spriteRenderer.sprite = spell.icon;
    }

    public void Interact(Interactor interactor)
    {
        Debug.Log("Interacted with " + name + ".");

        // Get a reference to the interactor's spell manager script
        SpellManager interactorSpellManager = interactor.GetComponentInChildren<SpellManager>();
        // Update the interactor's current spell to this spell
        interactorSpellManager.SwapSpell(spell);
        // Destroy the spell item
        Destroy(gameObject);
    }
}
