using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenAngelWingAnimation : MonoBehaviour
{
    public Animator animator;                 // Animator playing the animation
    public string animationName;             // Name of the animation to play
    public GameObject spriteToDisable;       // Sprite/GameObject to turn off after animation
    public GameObject spriteToEnable;        // Sprite/GameObject to turn on after animation

    private bool hasPlayed = false;

    void Start()
    {
        if (animator == null || string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("Animator or animationName not set.");
            return;
        }

        // Play the animation
        animator.Play(animationName);
        hasPlayed = true;
    }

    void Update()
    {
        if (!hasPlayed || animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the animation has finished
        if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f)
        {
            // Disable the current sprite
            if (spriteToDisable != null)
                spriteToDisable.SetActive(false);

            // Enable the next sprite
            if (spriteToEnable != null)
                spriteToEnable.SetActive(true);

            // Optional: disable this script so it doesn't run again
            this.enabled = false;
        }
    }
}
