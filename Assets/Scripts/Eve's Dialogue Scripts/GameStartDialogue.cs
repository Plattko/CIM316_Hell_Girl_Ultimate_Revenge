using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class GameStartDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation gameStartDialogue;

    private void Start()
    {
        StartDialogue();
    }

    private void StartDialogue()
    {
        //Check if dialogue panel is active
        if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active");
        }

        //Start dialogue
        ConversationManager.Instance.StartConversation(gameStartDialogue);
        Debug.Log("Dialogue has been triggered");
    }
}
