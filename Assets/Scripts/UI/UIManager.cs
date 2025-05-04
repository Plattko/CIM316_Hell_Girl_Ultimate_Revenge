using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private HUD hud;
    [SerializeField] private Minimap minimap;
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject endOfDemoMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }

        pauseMenu.SetActive(false);
        endOfDemoMenu.SetActive(false);
    }

    //-------------------------------------------------------------
    // UPDATING HUD
    //-------------------------------------------------------------
    public void UpdateHealth(int curHealth)
    {
        hud.UpdateHealth(curHealth);
    }

    public IEnumerator PlaySpellPickedUpAnim()
    {
        yield return hud.PlaySpellPickedUpAnim();
    }

    public IEnumerator PlaySpellOnboarding1Anim()
    {
        yield return hud.PlaySpellOnboarding1Anim();
    }

    public void PlaySpellOnboarding2Anim()
    {
        hud.PlaySpellOnboarding2Anim();
    }

    public void UpdateSpell(Spell newSpell)
    {
        hud.UpdateSpell(newSpell);
    }

    public void UpdateMana(int curMana)
    {
        hud.UpdateMana(curMana);
    }

    public void UpdateMap(Vector2Int newRoomPos, Room prevRoom)
    {
        minimap.UpdateMap(newRoomPos, prevRoom);
    }

    public void SetItemBanner(string itemName, string itemDescription)
    {
        hud.SetItemBanner(itemName, itemDescription);
    }

    public void FadeOutItemBanner()
    {
        StartCoroutine(hud.FadeOutItemBanner());
    }

    public void FadeInItemBanner()
    {
        StartCoroutine(hud.FadeInItemBanner());
    }

    public void SetBossHealthBar(float maxValue, float curValue)
    {
        hud.SetBossHealthBar(maxValue, curValue);
    }

    public void UpdateBossHealthBar(float value)
    {
        hud.UpdateBossHealthBar(value);
    }

    //-------------------------------------------------------------
    // HUD VISIBILITY
    //-------------------------------------------------------------
    public void ToggleHUDVisibility(bool enabled)
    {
        hud.ToggleHUDVisibility(enabled);
    }

    public void FadeOutHUD()
    {
        StartCoroutine(hud.FadeOutHUD());
    }

    public void FadeInHUD()
    {
        StartCoroutine(hud.FadeInHUD());
    }

    //-------------------------------------------------------------
    // FADE TO BLACK
    //-------------------------------------------------------------
    public IEnumerator FadeOut(float duration)
    {
        yield return fadeToBlack.FadeOut(duration);
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return fadeToBlack.FadeIn(duration);
    }

    public void ToggleBlack(bool enabled)
    {
        fadeToBlack.ToggleBlack(enabled);
    }

    //-------------------------------------------------------------
    // PAUSE MENU
    //-------------------------------------------------------------
    public void OpenPauseMenu()
    {
        ToggleHUDVisibility(false);
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        if (!GameManager.Instance.IsInOnboarding)
        {
            ToggleHUDVisibility(true);
        }
    }

    //-------------------------------------------------------------
    // DEATH MENU
    //-------------------------------------------------------------
    public void OpenDeathMenu()
    {
        ToggleHUDVisibility(false);
        deathMenu.SetActive(true);
    }

    //-------------------------------------------------------------
    // END OF DEMO MENU
    //-------------------------------------------------------------
    public void OpenEndOfDemoMenu()
    {
        ToggleHUDVisibility(false);
        endOfDemoMenu.SetActive(true);
    }
}
