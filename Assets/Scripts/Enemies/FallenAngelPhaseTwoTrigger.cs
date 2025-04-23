using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenAngelPhaseTwoTrigger : MonoBehaviour
{
    [Header("Health Source")]
    [SerializeField] private FallenAngel fallenAngel;

    [Header("Transition Settings")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private string animationTriggerName = "Phase2";

    [Header("Object Switching")]
    [SerializeField] private GameObject objectToDisable;
    [SerializeField] private GameObject objectToEnable;

    private bool hasTriggered = false;

    void Update()
    {
        if (fallenAngel == null) return;

        float currentHealth = fallenAngel.GetCurrentHealth();
        float maxHealth = fallenAngel.maxHealth;

        if (!hasTriggered && currentHealth <= maxHealth * 0.5f)
        {
            TriggerPhaseTwo();
        }
    }

    void TriggerPhaseTwo()
    {
        hasTriggered = true;

        if (transitionAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            transitionAnimator.SetTrigger(animationTriggerName);
            Debug.Log("Phase 2 animation triggered.");
        }

        if (objectToDisable != null) objectToDisable.SetActive(false);
        if (objectToEnable != null) objectToEnable.SetActive(true);
    }
}
