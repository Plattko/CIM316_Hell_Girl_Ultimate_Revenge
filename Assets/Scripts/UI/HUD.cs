using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Item Banner")]
    [SerializeField] private CanvasGroup itemBannerGroup;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private float itemBannerFadeOutDuration = 0.15f;
    [SerializeField] private float itemBannerFadeInDuration = 0.15f;

    [Header("Boss Health Bar")]
    [SerializeField] private Slider bossHealthBar;

    [Header("Visibility")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator anim;

    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeInDuration = 0.25f;

    private void Start()
    {
        itemBannerGroup.alpha = 0f;
    }

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

    public void SetItemBanner(string itemName, string itemDescription)
    {
        itemNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
    }

    public IEnumerator FadeOutItemBanner()
    {
        yield return FadeItemBanner(1, 0, itemBannerFadeOutDuration);
    }

    public IEnumerator FadeInItemBanner()
    {
        yield return FadeItemBanner(0, 1, itemBannerFadeInDuration);
    }

    private IEnumerator FadeItemBanner(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0;

        // Lerp from the start colour to the end colour over the fade's duration
        while (elapsedTime < duration)
        {
            itemBannerGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        itemBannerGroup.alpha = endAlpha;
    }

    //-------------------------------------------------------------
    // BOSS HEALTH BAR
    //-------------------------------------------------------------
    public void SetBossHealthBar(float maxValue, float curValue)
    {
        bossHealthBar.maxValue = maxValue;
        bossHealthBar.value = curValue;
    }

    public void UpdateBossHealthBar(float value)
    {
        bossHealthBar.value = value;
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
        yield return Fade(1, 0, fadeOutDuration);
    }

    public IEnumerator FadeInHUD()
    {
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
