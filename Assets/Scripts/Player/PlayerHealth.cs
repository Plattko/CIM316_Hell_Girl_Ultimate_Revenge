using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    public int curHealth { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioClip[] hurtSFX;

    // TEMPORARY
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DamageFlash damageFlash;

    private void Start()
    {
        // Set the player's current health to their max health
        curHealth = maxHealth;
        // Update the health UI
        UIManager.Instance.UpdateHealth(curHealth);
    }

    //private void Update()
    //{
    //    // Debug key to deal 1 damage to the player
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        TakeDamage(1);
    //    }
    //    // Debug key to restore 1 health for the player
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        Heal(1);
    //    }
    //}

    public void TakeDamage(float amount)
    {
        // Do nothing if the player is dashing
        if (playerController.isDashing) return;

        // Onboarding functionality
        if (GameManager.Instance.IsInOnboarding)
        {
            // Play the hurt SFX
            SFXManager.Instance.PlayRandomAudioClip(hurtSFX, transform, 0.75f, 1f, true);
            // Play the damage flash
            damageFlash.DoDamageFlash();
            // Do nothing else
            return;
        }

        // Decrease the health by the damage amount
        curHealth -= Mathf.RoundToInt(amount);
        // Play the damage flash
        damageFlash.DoDamageFlash();
        // Play the hurt SFX if the player is still alive
        if (curHealth > 0)
        {
            SFXManager.Instance.PlayRandomAudioClip(hurtSFX, transform, 0.75f, 1f, true);
        }
        // Restart the scene if the player reaches 0 health
        else
        {
            GameManager.Instance.GameOver();
        }
        // Update the health UI
        UIManager.Instance.UpdateHealth(curHealth);
    }

    public void Heal(float amount)
    {
        // Increase the health by the healing amount
        curHealth += Mathf.RoundToInt(amount);
        // Prevent the health from exceeding the max health
        if (curHealth > maxHealth) { curHealth = maxHealth; }
        // Update the health UI
        UIManager.Instance.UpdateHealth(curHealth);
    }
}
