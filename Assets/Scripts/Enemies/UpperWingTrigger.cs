using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperWingTrigger : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float triggerPercentage = 0.75f;

    [Header("Target Script Reference")]
    [SerializeField] private MonoBehaviour targetScript; // Drag the exact script component here

    private float currentHealth;
    private bool hasTriggered = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (targetScript != null)
        {
            targetScript.enabled = false;
        }
        else
        {
            Debug.LogWarning("Target script reference is not assigned.");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Enemy took {amount} damage. Health: {currentHealth}");

        if (!hasTriggered && currentHealth <= maxHealth * triggerPercentage)
        {
            EnableTargetScript();
        }
    }

    private void EnableTargetScript()
    {
        if (targetScript != null)
        {
            targetScript.enabled = true;
            hasTriggered = true;
            Debug.Log("Target script enabled.");
        }
        else
        {
            Debug.LogWarning("Target script is null when trying to enable it.");
        }
    }
}
