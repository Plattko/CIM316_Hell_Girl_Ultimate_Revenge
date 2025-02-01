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
        spellManager = player.GetComponentInChildren<SpellManager>();
    }

    private void OnEnable()
    {
        if (spellManager != null)
        {
            spellManager.onManaUpdated += UpdateManaUI;
            spellManager.onSpellUpdated += UpdateSpellUI;
        }
        else
        {
            Debug.LogWarning("No Spell Manager detected.");
        }
    }

    private void OnDisable()
    {
        if (spellManager != null)
        {
            spellManager.onManaUpdated -= UpdateManaUI;
            spellManager.onSpellUpdated -= UpdateSpellUI;
        }
        else
        {
            Debug.LogWarning("No Spell Manager detected.");
        }
    }

    private void Start()
    {
        //Debug.Log("this script is working");
        playerCharacter.InitializeHealth();
        UpdateHeartUI();
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
        //Debug.Log("hearts updating");
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
