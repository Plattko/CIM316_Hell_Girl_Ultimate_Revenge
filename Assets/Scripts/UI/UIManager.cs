using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private HUD hud;
    [SerializeField] private Minimap minimap;
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject endOfDemoMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }

        pauseMenu.SetActive(false);
    }

    public void UpdateHealth(int curHealth)
    {
        hud.UpdateHealth(curHealth);
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

    public IEnumerator FadeOut()
    {
        yield return fadeToBlack.FadeOut();
    }

    public IEnumerator FadeIn()
    {
        yield return fadeToBlack.FadeIn();
    }

    public void OpenPauseMenu()
    {
        hud.gameObject.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        hud.gameObject.SetActive(true);
    }

    public void OpenEndOfDemoMenu()
    {
        hud.gameObject.SetActive(false);
        endOfDemoMenu.SetActive(true);
    }
}
