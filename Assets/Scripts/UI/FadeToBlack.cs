using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeInDuration = 0.1f;

    private Color transparent = new Color(0, 0, 0, 0);
    private Color black = Color.black;

    public IEnumerator FadeOut()
    {
        gameObject.SetActive(true);
        yield return Fade(transparent, black, fadeOutDuration);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(black, transparent, fadeInDuration);
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(Color startColour, Color endColour, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            fadeImage.color = Color.Lerp(startColour, endColour, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
