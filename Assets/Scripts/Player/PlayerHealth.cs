using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    public int curHealth { get; private set; }

    private void Start()
    {
        // Set the player's current health to their max health
        curHealth = maxHealth;
        // Update the health UI
        UIManager.Instance.UpdateHealth(curHealth);
    }

    private void Update()
    {
        // Debug key to deal 1 damage to the player
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(1);
        }
        // Debug key to restore 1 health for the player
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(1);
        }
    }

    public void TakeDamage(float amount)
    {
        // Decrease the health by the damage amount
        curHealth -= Mathf.RoundToInt(amount);
        // Restart the scene if the player reaches 0 health
        if (curHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
