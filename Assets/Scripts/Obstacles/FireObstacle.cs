using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObstacle : MonoBehaviour, IDamageable
{
    [SerializeField] private AudioClip extinguishSFX;
    
    public void TakeDamage(float damage)
    {
        SFXManager.Instance.PlayAudioClip(extinguishSFX, transform, 0.3f, 1f, true);
        Destroy(gameObject);
    }
}
