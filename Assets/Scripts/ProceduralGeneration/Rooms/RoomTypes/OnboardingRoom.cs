using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class OnboardingRoom : MonoBehaviour
{
    // Gate references
    [SerializeField] private GameObject[] gates = new GameObject[4];

    [SerializeField] private ConversationManager conversationManager;
    [SerializeField] private TutorialFight tutorialFight;
    private int dialogueIndex = 0;

    // Knife reference
    [SerializeField] private Item knifeItem;

    // SFX variables
    [SerializeField] private AudioClip gateOpenSFX;

    private void OnEnable()
    {
        conversationManager.OnDialogueEnd += OnDialogueEnd;
    }

    private void OnDisable()
    {
        conversationManager.OnDialogueEnd -= OnDialogueEnd;
    }

    private void Start()
    {
        // Set the gates to closed
        foreach (GameObject gate in gates)
        {
            if (gate.activeInHierarchy)
            {
                gate.GetComponent<Animator>().Play("Gate_Closed");
            }
        }
        // Toggle on onboarding mode to prevent the player from taking damage
        GameManager.Instance.ToggleOnboardingMode(true);
        // Hide the HUD
        UIManager.Instance.ToggleHUDVisibility(false);
        // Set the screen to black
        UIManager.Instance.ToggleBlack(true);
    }

    public void GiveHelenaKnife()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(playerController.StartItemPickup(knifeItem, false));
    }

    public void EndItemPickup()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.EndItemPickup();
    }

    public void OpenGates()
    {
        // Open the gates
        foreach (GameObject gate in gates)
        {
            if (gate.activeInHierarchy)
            {
                gate.GetComponent<Animator>().Play("Gate_Open");
            }
        }
        // Play the gate open SFX
        SFXManager.Instance.PlayAudioClip(gateOpenSFX, transform, 1f);
    }
    public void FadeIn()
    {
        // Fade in from black screen
        UIManager.Instance.StartCoroutine(UIManager.Instance.FadeIn(0.25f));
    }

    private void OnDialogueEnd()
    {
        // Increment the dialogue index
        dialogueIndex++;
        // If it's the end of the first conversation, spawn the enemy
        if (dialogueIndex == 1)
        {
            tutorialFight.SpawnEnemy();
        }
        // If it's the end of the second conversation, fade in the HUD and disable onboarding mode
        else if (dialogueIndex == 2)
        {
            UIManager.Instance.FadeInHUD();
            GameManager.Instance.ToggleOnboardingMode(false);
            StatsManager.Instance.ToggleRunTimer(true);
        }
    }
}
