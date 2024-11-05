using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerCharacter playerCharacter;

    [Header("Health")]
    public Image[] heartImages;  // Array of Image components to show hearts

    void Start()
    {
        Debug.Log("this script is working");
        playerCharacter.InitializeHealth();
        UpdateHeartUI();
    }

    // Call this whenever the player's health changes
    public void UpdateHeartUI()
    {
        Debug.Log("hearts updating");
        Sprite[] hearts = playerCharacter.GetHearts();

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = hearts[i];
        }
    }

    void Update()
    {
        // Simulating damage with space bar for testing
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Damage is being done");
            playerCharacter.TakeDamage(1);
            UpdateHeartUI();
        }
    }
}
