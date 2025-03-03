using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerCharacter playerCharacter;
    [SerializeField] private GameObject player;
    private SpellManager spellManager;

    [Header("Health")]
    public Image[] heartImages;  // Array of Image components to show hearts

    [Header("Spells")]
    [SerializeField] private Image spellIcon;
    [SerializeField] private Transform manaIcons;

    private void Awake()
    {
        if (player != null)
        {
            spellManager = player.GetComponentInChildren<SpellManager>();
            Debug.Log("Spell Manager set in Awake.");
        }
    }

    private void OnEnable()
    {
        if (spellManager != null)
        {
            spellManager.onManaUpdated += UpdateManaUI;
            spellManager.onSpellUpdated += UpdateSpellUI;
            Debug.Log("Spell Manager events subscribed to in OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (spellManager != null)
        {
            spellManager.onManaUpdated -= UpdateManaUI;
            spellManager.onSpellUpdated -= UpdateSpellUI;
            Debug.Log("Spell Manager events unsubscribed from in OnDisable.");
        }
    }

    private void Start()
    {
        if (player != null)
        {
            playerCharacter.InitializeHealth();
            UpdateHeartUI();
            Debug.Log("Player health initialised in Start.");
        }
    }

    public void Initialise(GameObject player)
    {
        // Set the spell manager to the player's spell manager script
        spellManager = player.GetComponentInChildren<SpellManager>();
        // Subscribe to the spell manager's events
        if (spellManager != null)
        {
            spellManager.onManaUpdated += UpdateManaUI;
            spellManager.onSpellUpdated += UpdateSpellUI;
            Debug.Log("Spell Manager events subscribed to in Initialise.");
        }

        // Initialise the player's health
        playerCharacter.InitializeHealth();
        // Update the heart UI to correspond with the player's health
        UpdateHeartUI();
        Debug.Log("Player health initialised in Initialise.");
    }

    void Update()
    {
        //// Simulating damage with space bar for testing
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Debug.Log("Damage is being done");
        //    playerCharacter.TakeDamage(1);
        //    UpdateHeartUI();
        //}
    }

    // Call this whenever the player's health changes
    public void UpdateHeartUI()
    {
        Sprite[] hearts = playerCharacter.GetHearts();

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = hearts[i];
        }
    }

    private void UpdateManaUI(int curMana)
    {
        Debug.Log("Updated mana: " +  curMana);

        // Show a number of mana icons equal to the player's current mana
        for (int i = 0; i < manaIcons.childCount; i++)
        {
            if (i < curMana)
            {
                manaIcons.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                manaIcons.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void UpdateSpellUI(Spell newSpell)
    {
        // Show the spell icon if it is hidden
        if (!spellIcon.enabled)
        {
            spellIcon.enabled = true;
        }
        // Set the spell icon to the new spell's icon
        spellIcon.sprite = newSpell.icon;
    }
}
