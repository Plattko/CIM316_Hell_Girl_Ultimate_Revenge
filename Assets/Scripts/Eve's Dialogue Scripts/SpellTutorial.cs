using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class SpellTutorial : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation beginingDialogue;
    [SerializeField] private NPCConversation finalDialogue;
    [SerializeField] private GameObject TDAchara;
    private PlayerController playerController;

    private SpellManager spellManager;

    public bool hasAlreadySpoken = false;

    private bool spellTutComplete = false;

    private void Start()
    {
        spellManager = FindAnyObjectByType<SpellManager>();

        spellTutComplete = PlayerPrefs.GetInt("SpellTutComplete", 0) == 1;

        if(spellTutComplete)
        {
            TDAchara.SetActive(false);
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        ActivateDialogue();
    //    }
    //}

    public void Interact(Interactor interactor)
    {
        if (!hasAlreadySpoken)
        {
            ActivateFirstDialogue();
        }
        else if (hasAlreadySpoken && spellManager.curSpell != null)
        {
            ActivateFinalDialogue();
        }
    }

    private void ActivateFirstDialogue()
    {
        //Check if dialogue panel is active
        if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active");
        }

        //Disable player movement 
        playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.ToggleMovement(false);
        }

        //Start dialogue
        ConversationManager.Instance.StartConversation(beginingDialogue);
        Debug.Log("Dialogue has been triggered");

        hasAlreadySpoken = true;
    }

    private void ActivateFinalDialogue()
    {
        //Check if dialogue panel is active
        if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active");
        }

        //Disable player movement 
        playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.ToggleMovement(false);
        }

        //Start dialogue
        ConversationManager.Instance.StartConversation(finalDialogue);
        Debug.Log("Dialogue has been triggered");

        spellTutComplete = true;
        PlayerPrefs.SetInt("SpellTutComplete", 1);
    }
}