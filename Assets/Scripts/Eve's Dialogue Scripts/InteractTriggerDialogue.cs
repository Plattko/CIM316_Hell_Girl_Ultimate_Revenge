using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class InteractTriggerDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation interactDialogue;
    private PlayerController playerController;


    //private void OnTriggerStay(Collider other)
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        ActivateDialogue();
    //    }
    //}

    public void Interact(Interactor interactor)
    {
        ActivateDialogue();
    }

    private void ActivateDialogue()
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
            playerController.DisableMovement();
        }

        //Start dialogue
        ConversationManager.Instance.StartConversation(interactDialogue);
        Debug.Log("Dialogue has been triggered");
    }
}
