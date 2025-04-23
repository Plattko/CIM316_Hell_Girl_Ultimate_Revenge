using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Sprite heartIconFull;
    [SerializeField] private Sprite heartIconEmpty;
    [SerializeField] private RectTransform heartIcons;

    [Header("Spells")]
    [SerializeField] private Image spellIcon;
    [SerializeField] private RectTransform manaIcons;
    private bool spellAnimHasPlayed;

    // Visibility
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator anim;

    public float fadeOutDuration = 0.25f;
    public float fadeInDuration = 0.25f;

    public void UpdateHealth(int curHealth)
    {
        for (int i = 0; i < heartIcons.childCount; i++)
        {
            // Show a number of full health icons equal to the player's current health
            if (i < curHealth)
            {
                heartIcons.GetChild(i).GetComponent<Image>().sprite = heartIconFull;
            }
            // Make the rest of the heart icons empty
            else
            {
                heartIcons.GetChild(i).GetComponent<Image>().sprite = heartIconEmpty;
            }
        }
    }

    public IEnumerator PlaySpellPickedUpAnim()
    {
        anim.Play("HUD_SpellPickedUp");

        while (!spellAnimHasPlayed)
        {
            yield return null;
        }
    }

    public void OnSpellAnimPlayed()
    {
        spellAnimHasPlayed = true;
    }

    public void UpdateMana(int curMana)
    {
        // Show a number of mana icons equal to the player's current mana
        for (int i = 0; i < manaIcons.childCount; i++)
        {
            // Enable a number of mana icons equal to the player's current mana
            if (i < curMana)
            {
                manaIcons.GetChild(i).GetComponent<Image>().enabled = true;
            }
            // Disable the rest
            else
            {
                manaIcons.GetChild(i).GetComponent<Image>().enabled = false;
            }
        }
    }

    public void UpdateSpell(Spell newSpell)
    {
        // Set the spell icon to the new spell's icon
        spellIcon.sprite = newSpell.icon;
    }

    //-------------------------------------------------------------
    // VISIBILITY
    //-------------------------------------------------------------
    public void ToggleHUDVisibility(bool enabled)
    {
        canvasGroup.alpha = enabled ? 1 : 0;
    }

    public IEnumerator FadeOutHUD()
    {
        // Fade from transparent to black over the fade out duration
        yield return Fade(1, 0, fadeOutDuration);
    }

    public IEnumerator FadeInHUD()
    {
        // Fade from black to transparent over the fade in duration
        yield return Fade(0, 1, fadeInDuration);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0;

        // Lerp from the start colour to the end colour over the fade's duration
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
