using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokePuff : MonoBehaviour
{
    [SerializeField] private bool isInstantiated = true;
    
    private void Start()
    {
        if (!isInstantiated) return;

        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
        particleSystem.Play();

        Destroy(gameObject, 0.5f);
    }
}
