using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathChunks : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip[] chunksSFX;
    private ParticleSystem chunksVFX;
    
    private void Start()
    {
        SFXManager.Instance.PlayRandomAudioClip(chunksSFX, transform, 0.75f, 0.8f, true);
        SFXManager.Instance.PlayRandomAudioClip(chunksSFX, transform, 0.75f, 1f, true);
        SFXManager.Instance.PlayRandomAudioClip(chunksSFX, transform, 0.75f, 1.2f, true);

        chunksVFX = GetComponent<ParticleSystem>();
        chunksVFX.Stop();
        chunksVFX.Play();

        Destroy(gameObject, 2.5f);
    }
}
