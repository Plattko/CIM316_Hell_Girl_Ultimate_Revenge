using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MapGenerator mapGenerator;

    public bool IsGamePaused { get; private set; }
    private bool canPause = true;

    [field: SerializeField] public bool HasOnboardedPlayer { get; private set; }
    public bool IsInOnboarding { get; private set; }

    public bool PlayerHasSpell { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        }
        else 
        { 
            Instance = this;
        }
    }

    //-------------------------------------------------------------
    // PAUSING
    //-------------------------------------------------------------
    public void TogglePause()
    {
        if (!canPause) return;
        
        if (!IsGamePaused)
        {
            OpenPauseMenu();
        }
        else
        {
            ClosePauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        IsGamePaused = true;
        StatsManager.Instance.ToggleRunTimer(false);
        Time.timeScale = 0;
        UIManager.Instance.OpenPauseMenu();
    }

    public void ClosePauseMenu()
    {
        IsGamePaused = false;
        StatsManager.Instance.ToggleRunTimer(true);
        Time.timeScale = 1;
        UIManager.Instance.ClosePauseMenu();
    }

    public void TogglePauseInput(bool enabled)
    {
        canPause = enabled;
    }

    //-------------------------------------------------------------
    // DYING
    //-------------------------------------------------------------
    public void GameOver()
    {
        canPause = false;
        IsGamePaused = true;
        StatsManager.Instance.ToggleRunTimer(false);
        Time.timeScale = 0;
        UIManager.Instance.OpenDeathMenu();
    }

    //-------------------------------------------------------------
    // QUITTING
    //-------------------------------------------------------------
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //-------------------------------------------------------------
    // ONBOARDING
    //-------------------------------------------------------------
    public void ToggleOnboardingMode(bool enabled)
    {
        IsInOnboarding = enabled;

        if (!enabled)
        {
            HasOnboardedPlayer = true;
        }
    }

    //-------------------------------------------------------------
    // RESTARTING/GENERATING NEW MAP
    //-------------------------------------------------------------
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void GenerateNewMap()
    {
        mapGenerator.StartCoroutine(mapGenerator.GenerateNewMap());
    }

    //-------------------------------------------------------------
    // END OF DEMO
    //-------------------------------------------------------------
    public void EndDemo()
    {
        canPause = false;
        IsGamePaused = true;
        StatsManager.Instance.ToggleRunTimer(false);
        Time.timeScale = 0;
        UIManager.Instance.OpenEndOfDemoMenu();
    }

    //-------------------------------------------------------------
    // Player
    //-------------------------------------------------------------
    public void TogglePlayerHasSpell(bool hasSpell)
    {
        PlayerHasSpell = hasSpell;
    }
}
