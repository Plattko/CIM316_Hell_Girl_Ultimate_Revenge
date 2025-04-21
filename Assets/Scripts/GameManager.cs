using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MapGenerator mapGenerator;

    private bool isGamePaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }

    public void TogglePause()
    {
        if (!isGamePaused)
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
        isGamePaused = true;
        Time.timeScale = 0;
        UIManager.Instance.OpenPauseMenu();
    }

    public void ClosePauseMenu()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        UIManager.Instance.ClosePauseMenu();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void GenerateNewMap()
    {
        mapGenerator.StartCoroutine(mapGenerator.GenerateNewMap());
    }

    public void EndDemo()
    {
        Time.timeScale = 0;
        UIManager.Instance.OpenEndOfDemoMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
