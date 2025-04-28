using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] ambiences;
    private AudioSource audioSource;

    private float maxVolume = 0.75f;

    private float fadeInDuration = 15f;
    private float fadeOutDuration = 15f;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    private float minWait = 15f;
    private float maxWait = 45f;
    private float nextAmbienceTime = float.MaxValue;

    private float endEarlyChance = 0.5f;
    private float endEarlyChanceFreq = 40f;
    private float nextEndEarlyChance = float.MaxValue;

    private float maxDuration = 120f;
    private float nextMaxDurationEnd = float.MaxValue;

    private void Start()
    {
        // Get a reference to the audio source
        audioSource = GetComponent<AudioSource>();
        // Subtract the fade out duration from the max duration
        maxDuration -= fadeOutDuration;
        // Start playing an ambience
        PlayAmbience();
    }

    private void Update()
    {
        // If it reaches the next ambience time, play an ambience
        if (Time.time > nextAmbienceTime)
        {
            Debug.Log("Reached next ambience time.");
            PlayAmbience();
        }

        // If the ambience is fading out, end here
        if (fadeOutCoroutine != null) return;

        // If it reaches the next early end chance, have a chance to end the current ambience early
        if (Time.time > nextEndEarlyChance)
        {
            Debug.Log("Reached next end early chance.");
            TryEndAmbienceEarly();
        }

        // If it reaches the max duration, end the current ambience
        if (Time.time > nextMaxDurationEnd)
        {
            Debug.Log("Reached max duration.");
            MaxDurationEnd();
        }
    }

    private void PlayAmbience()
    {
        Debug.Log("Ambience played.");
        // Choose a random audio clip
        AudioClip audioClip = ambiences[Random.Range(0, ambiences.Length)];
        // Set the next ambience time to the audio clip's length plus a random number of seconds between the min and max wait time
        nextAmbienceTime = Time.time + audioClip.length + Random.Range(minWait, maxWait);
        // Set the audio source's clip to the clip
        audioSource.clip = audioClip;
        // Reset the audio source's volume to 0
        audioSource.volume = 0;
        // Play the audio source
        audioSource.Play();
        // Fade the volume in
        fadeInCoroutine = StartCoroutine(FadeInAmbience());
        // Set the next early end chance to now plus the early end chance frequency
        nextEndEarlyChance = Time.time + endEarlyChanceFreq;
        // Set the next max duration end to now plus the max duration
        nextMaxDurationEnd = Time.time + maxDuration;
    }

    private void EndAmbience()
    {
        Debug.Log("Ambience ended.");
        // Fade out the ambience
        fadeOutCoroutine = StartCoroutine(FadeOutAmbience());
        // Stop playing the audio source after fading the ambience out
        Invoke("StopAudioSource", fadeOutDuration);
    }
    
    private void StopAudioSource()
    {
        audioSource.Stop();
    }

    private void TryEndAmbienceEarly()
    {
        Debug.Log("Tried to end ambience early.");
        // Have a chance of ending early, otherwise reset the next end early chance
        if (Random.value < endEarlyChance)
        {
            Debug.Log("Ambience ended early.");
            // Set the next early end chance and next max duration end to the max value so it doesn't trigger until a new ambience is played
            nextEndEarlyChance = float.MaxValue;
            nextMaxDurationEnd = float.MaxValue;
            // Subtract the remaining clip duration from the next ambience time
            // to ensure the wait before the next clip is between the min and max wait time
            float remainingClipTime = audioSource.clip.length - (audioSource.time + fadeOutDuration);
            nextAmbienceTime -= remainingClipTime;
            // End the ambience
            EndAmbience();
        }
        else
        {
            Debug.Log("Ambience not ended early.");
            // Set the next early end chance to now plus the early end chance frequency
            nextEndEarlyChance = Time.time + endEarlyChanceFreq;
        }
    }

    private void MaxDurationEnd()
    {
        Debug.Log("Ended from max duration.");
        // Set the next early end chance and next max duration end to the max value so it doesn't trigger until a new ambience is played
        nextEndEarlyChance = float.MaxValue;
        nextMaxDurationEnd = float.MaxValue;
        // Subtract the max duration plus the fade out time from the next ambience time
        // to ensure the wait before the next clip is between the min and max wait time
        nextAmbienceTime -= maxDuration + fadeOutDuration;
        // End the ambience
        EndAmbience();
    }

    public IEnumerator FadeInAmbience()
    {
        // Fade from full to 0 volume over the fade in duration
        yield return FadeAmbience(0f, maxVolume, fadeInDuration);
    }

    public IEnumerator FadeOutAmbience()
    {
        // Fade from 0 to full volume over the fade out duration
        yield return FadeAmbience(maxVolume, 0f, fadeOutDuration);
    }

    private IEnumerator FadeAmbience(float startVolume, float endVolume, float duration)
    {
        float elapsedTime = 0;

        // Lerp from the start volume to the end volume over the fade's duration
        while (elapsedTime < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        // Ensure the audio source reaches the end volume
        audioSource.volume = endVolume;

        // Nullify the coroutine
        NullifyCoroutine(startVolume, endVolume);
    }

    private void NullifyCoroutine(float startVolume, float endVolume)
    {
        if (startVolume > endVolume)
        {
            fadeOutCoroutine = null;
        }
        else
        {
            fadeInCoroutine = null;
        }
    }
}
