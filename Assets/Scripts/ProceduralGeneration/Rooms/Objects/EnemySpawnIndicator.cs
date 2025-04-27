using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnIndicator : MonoBehaviour
{
    [SerializeField] private AudioClip spawnSFX;
    
    private void Start()
    {
        SFXManager.Instance.PlayAudioClip(spawnSFX, transform, 1f, 1f, true);
    }
}
