using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSourcePrefab;

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

    public void PlayAudioClip(AudioClip audioClip, Transform spawnTransform, float volume, float pitch = 1f, bool randomisePitch = false, float pitchRange = 0.1f)
    {
        // Spawn the audio source game object and get a reference to its AudioSource component
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnTransform.position, Quaternion.identity);
        // Assign the audio clip
        audioSource.clip = audioClip;
        // Assign the volume
        audioSource.volume = volume;
        // Assign the base pitch
        audioSource.pitch = pitch;
        // Assign the pitch if it's randomised
        if (randomisePitch)
        {
            // Give the pitch slight randomness to make it less repetitive
            audioSource.pitch = Random.Range(audioSource.pitch - pitchRange, audioSource.pitch + pitchRange);
        }
        // Play the audio clip
        audioSource.Play();
        // Get the length of the audio clip
        float clipLength = audioSource.clip.length;
        // Destroy the audio source when the clip ends
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomAudioClip(AudioClip[] audioClips, Transform spawnTransform, float volume, float pitch = 1f, bool randomisePitch = false, float pitchRange = 0.1f)
    {
        // Spawn the audio source game object and get a reference to its AudioSource component
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnTransform.position, Quaternion.identity);
        // Assign a random audio clip from the array of audio clips
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        // Assign the volume
        audioSource.volume = volume;
        // Assign the base pitch
        audioSource.pitch = pitch;
        // Assign the pitch if it's randomised
        if (randomisePitch)
        {
            // Give the pitch slight randomness to make it less repetitive
            audioSource.pitch = Random.Range(audioSource.pitch - pitchRange, audioSource.pitch + pitchRange);
        }
        // Play the audio clip
        audioSource.Play();
        // Get the length of the audio clip
        float clipLength = audioSource.clip.length;
        // Destroy the audio source when the clip ends
        Destroy(audioSource.gameObject, clipLength);
    }
}
