using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeObstacle : MonoBehaviour
{
    private Animator anim;

    private bool canTrigger = true;

    [Header("SFX")]
    [SerializeField] private AudioClip spikeSFX;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Trigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Trigger(other);
    }

    private void Trigger(Collider other)
    {
        if (!canTrigger) return;

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            canTrigger = false;

            anim.SetTrigger("trigger");
        }
    }

    public void EnableTrigger()
    {
        canTrigger = true;
    }

    public void PlaySpikeSFX()
    {
        SFXManager.Instance.PlayAudioClip(spikeSFX, transform, 1f, 1f, true);
    }
}
