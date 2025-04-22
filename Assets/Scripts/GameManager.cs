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

    public bool IsInOnboarding { get; private set; }

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
        Time.timeScale = 0;
        UIManager.Instance.OpenPauseMenu();
    }

    public void ClosePauseMenu()
    {
        IsGamePaused = false;
        Time.timeScale = 1;
        UIManager.Instance.ClosePauseMenu();
    }

    public void TogglePauseInput(bool enabled)
    {
        canPause = enabled;
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
        Time.timeScale = 0;
        UIManager.Instance.OpenEndOfDemoMenu();
    }
}
