using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeStatText;
    [SerializeField] private TextMeshProUGUI floorReachedStatText;
    [SerializeField] private TextMeshProUGUI roomsClearedStatText;
    [SerializeField] private TextMeshProUGUI enemiesKilledStatText;
    [SerializeField] private TextMeshProUGUI itemsPickedUpStatText;

    private void OnEnable()
    {
        DisplayStats();
    }

    public void DisplayStats()
    {
        timeStatText.text = StatsManager.Instance.RunTime.ToString("hh':'mm':'ss");
        floorReachedStatText.text = StatsManager.Instance.FloorReached.ToString();
        roomsClearedStatText.text = StatsManager.Instance.RoomsCleared.ToString();
        enemiesKilledStatText.text = StatsManager.Instance.EnemiesKilled.ToString();
        itemsPickedUpStatText.text = StatsManager.Instance.ItemsPickedUp.ToString();
    }
}
