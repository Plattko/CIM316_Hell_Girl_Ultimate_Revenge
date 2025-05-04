using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private FadeToBlack fadeToBlack;

    [SerializeField] private AudioClip loadSFX;
    
    public void PlayButton()
    {
        StartCoroutine(StartGameSequence());
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    private IEnumerator StartGameSequence()
    {
        PlayerPrefs.DeleteAll();

        SFXManager.Instance.PlayAudioClip(loadSFX, transform, 0.5f);
        yield return fadeToBlack.FadeOut(0.5f);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(1);
    }
}
