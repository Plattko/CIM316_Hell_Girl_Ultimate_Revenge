using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    // References
    [SerializeField] private SpriteRenderer sr;

    public void Init(bool isSpriteFlipped, Color startColour, float startAlpha, float duration)
    {
        // Set whether the sprite is flipped
        sr.flipX = isSpriteFlipped;
        // Set the afterimage's colour to the start colour with the start alpha
        sr.color = new Color(startColour.r, startColour.g, startColour.b, startAlpha);
        // Start fading out
        StartCoroutine(Fade(duration));
    }

    private IEnumerator Fade(float duration)
    {
        // Set the start colour to the afterimage's current colour
        Color startColour = sr.color;
        // Set the end colour to the start colour with 0 alpha
        Color endColour = new Color(startColour.r, startColour.g, startColour.b, 0f);

        float elapsedTime = 0;

        // Lerp the afterimage's colour from the start colour to the end colour over the fade duration
        while (elapsedTime < duration)
        {
            sr.color = Color.Lerp(startColour, endColour, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure it reaches the end colour
        sr.color = endColour;
        // Destroy the afterimage
        Destroy(gameObject);
    }
}
