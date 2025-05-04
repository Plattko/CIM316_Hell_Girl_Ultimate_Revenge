using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class SpellOnboardingRoom : MonoBehaviour
{
    [SerializeField] private List<Item> weaponPool = new List<Item>();
    [SerializeField] private List<Item> spellPool = new List<Item>();
    [SerializeField] private ItemPedestal[] itemPedestals;

    public enum ItemType
    {
        Weapons,
        Spells,
    }
    public ItemType itemType { get; private set; } = ItemType.Weapons;

    // Onboarding variables
    private ConversationManager conversationManager;
    [SerializeField] private GameObject[] gates = new GameObject[4];
    [SerializeField] private SpriteRenderer[] itemDisplays = new SpriteRenderer[2];
    [SerializeField] private ParticleSystem[] smokePuffs = new ParticleSystem[2];

    private SpellManager playerSpellManager;

    [Header("SFX")] 
    [SerializeField] private AudioClip gateCloseSFX;
    [SerializeField] private AudioClip gateOpenSFX;

    private int dialogueIndex = 0;

    private bool isRoomCleared;

    private void Awake()
    {
        conversationManager = GameObject.FindGameObjectWithTag("ConversationManager").GetComponent<ConversationManager>();
    }

    private void Start()
    {
        playerSpellManager = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SpellManager>();
    }

    private void OnEnable()
    {
        // Connect every item pedestals's onItemChosen event to the ItemChosen function
        foreach (ItemPedestal pedestal in itemPedestals)
        {
            pedestal.onItemChosen += OnItemChosen;
        }

        conversationManager.OnDialogueEnd += OnDialogueEnd;
    }

    private void OnDisable()
    {
        // Disconnect every item pedestals's onItemChosen event from the ItemChosen function
        foreach (ItemPedestal pedestal in itemPedestals)
        {
            pedestal.onItemChosen -= OnItemChosen;
        }

        if (conversationManager != null)
        {
            conversationManager.OnDialogueEnd -= OnDialogueEnd;
        }
    }

    public void InitialiseRoom()
    {
        // If the room hasn't been cleared, close the gates and spawn a wave of enemies
        if (!isRoomCleared)
        {
            // Close the gates to prevent the player from leaving
            foreach (GameObject gate in gates)
            {
                if (gate.activeInHierarchy)
                {
                    gate.GetComponent<Animator>().Play("Gate_Close");
                }
            }
            // Play the gate close SFX
            SFXManager.Instance.PlayAudioClip(gateCloseSFX, transform, 1f);
            // Hide the item displays
            foreach (SpriteRenderer itemDisplay in itemDisplays)
            {
                itemDisplay.enabled = false;
            }
        }
    }

    public void InitialisePedestals()
    {
        // Alternate between spell rooms and weapon rooms
        itemType = GameManager.Instance.DoSpellRoom ? ItemType.Spells : ItemType.Weapons;
        GameManager.Instance.ToggleItemRoom();
        // Choose between using the weapon pool or spell pool based on the room's item type
        List<Item> availableItems = new List<Item>();
        if (itemType == ItemType.Weapons) { availableItems = weaponPool; }
        else if (itemType == ItemType.Spells) { availableItems = spellPool; }
        // Give each pedestal an item from the list of available items without repetition
        foreach (ItemPedestal itemPedestal in itemPedestals)
        {
            // Roll a random item from the available items list
            int roll = Random.Range(0, availableItems.Count);
            // Initialise the pedestal with the rolled item
            itemPedestal.Initialise(availableItems[roll]);
            // Remove the item from the available items list
            availableItems.RemoveAt(roll);
            // Show the item displays
            foreach (SpriteRenderer itemDisplay in itemDisplays)
            {
                itemDisplay.enabled = true;
            }
        }
        // Play the smoke puff VFX
        foreach (ParticleSystem smokePuff in smokePuffs)
        {
            smokePuff.Stop();
            smokePuff.Play();
        }
    }

    private void OnItemChosen()
    {
        // Destroy each pedestal's item display
        foreach (ItemPedestal pedestal in itemPedestals)
        {
            // Unsubscribe from the pedestal's item chosen event
            pedestal.onItemChosen -= OnItemChosen;
            // Clear the pedestal so the item is no longer displayed and it can't be interacted with
            pedestal.Clear();
        }
    }

    public void RoomCleared()
    {
        // Set the room to cleared
        isRoomCleared = true;

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

    public void GivePlayerMana()
    {
        StartCoroutine(GivePlayerManaSequence());
    }

    private IEnumerator GivePlayerManaSequence()
    {
        UIManager.Instance.PlaySpellOnboarding2Anim();

        playerSpellManager.GainMana(1);
        yield return new WaitForSeconds(10f / 60f);
        playerSpellManager.GainMana(1);
        yield return new WaitForSeconds(10f / 60f);
        playerSpellManager.GainMana(1);
        yield return new WaitForSeconds(10f / 60f);
        playerSpellManager.GainMana(1);
        yield return new WaitForSeconds(10f / 60f);
        playerSpellManager.GainMana(1);
    }

    private void OnDialogueEnd()
    {
        // Increment the dialogue index
        dialogueIndex++;
        // If it's the end of the second conversation, clear the room
        if (dialogueIndex == 2)
        {
            RoomCleared();
        }
    }
}
