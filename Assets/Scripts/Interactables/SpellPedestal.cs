using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPedestal : MonoBehaviour, IInteractable
{
    public GameObject spellSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Spell spell;
    public event Action onSpellChosen;

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
        // Signal that the spell item was chosen
        onSpellChosen?.Invoke();
    }
}
