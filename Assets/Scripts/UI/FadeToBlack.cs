using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FadeToBlack : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public float fadeOutDuration = 0.25f;
    public float fadeInDuration = 0.1f;

    private Color transparent = new Color(0, 0, 0, 0);
    private Color black = Color.black;

    public IEnumerator FadeOut()
    {
        // Fade from transparent to black over the fade out duration
        yield return Fade(transparent, black, fadeOutDuration);
    }

    public IEnumerator FadeIn()
    {
        // Fade from black to transparent over the fade in duration
        yield return Fade(black, transparent, fadeInDuration);
    }

    private IEnumerator Fade(Color startColour, Color endColour, float duration)
    {
        float elapsedTime = 0;

        // Lerp from the start colour to the end colour over the fade's duration
        while (elapsedTime < duration)
        {
            fadeImage.color = Color.Lerp(startColour, endColour, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
