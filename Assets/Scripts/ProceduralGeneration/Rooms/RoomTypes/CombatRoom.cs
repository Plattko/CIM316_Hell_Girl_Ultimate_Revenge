using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : MonoBehaviour
{
    // Gate references
    [SerializeField] private GameObject[] gates = new GameObject[4];
    
    // Wave variables
    private bool isRoomCleared;
    private int wavesToClear;
    private int wavesCleared = 0;
    private float doubleWaveChance = 0.25f;
    private float newWaveDelay = 1f;

    // Enemy variables
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemySpawnIndicatorPrefab;
    [SerializeField] private GameObject[] enemySpawnLayouts;
    private List<GameObject> usedSpawnLayouts = new List<GameObject>();
    private int enemiesToKill;
    private int enemiesKilled = 0;

    // Health pickup variables
    [SerializeField] private GameObject healthPickupPrefab;
    private float heartSpawnChance = 0.33f;

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
            // Randomly set the number to waves to clear between 1 and 2 based on the double wave chance
            wavesToClear = Random.value < doubleWaveChance ? 2 : 1;
            // Spawn a wave
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        // Reset the enemies killed count
        enemiesKilled = 0;

        // Pick a random enemy spawn layout
        GameObject enemySpawnLayout = null;
        do
        {
            enemySpawnLayout = enemySpawnLayouts[Random.Range(0, enemySpawnLayouts.Length)];
        }
        // Repeat if the spawn layout has already been used
        while (usedSpawnLayouts.Contains(enemySpawnLayout));
        // Add the enemy spawn layout to the list of used spawn layouts
        usedSpawnLayouts.Add(enemySpawnLayout);
        // Set the number of enemies to kill to the number of spawn points in the spawn layout
        enemiesToKill = enemySpawnLayout.transform.childCount;

        // Display an enemy spawn indicator and then spawn an enemy at each of the spawn layout's spawn points
        foreach (Transform spawnPoint in enemySpawnLayout.transform)
        {
            // Instantiate an enemy spawn indicator at the position of the spawn point
            GameObject enemySpawnIndicator = Instantiate(enemySpawnIndicatorPrefab, spawnPoint.position, Quaternion.identity);
            // Spawn an enemy at the spawn point after the enemy spawn indicator's animation ends
            StartCoroutine(SpawnEnemy(enemySpawnIndicator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length, spawnPoint.position));
        }
    }

    private IEnumerator SpawnEnemy(float delay, Vector3 spawnPoint)
    {
        // Wait for the spawn delay
        yield return new WaitForSeconds(delay);
        // Instantiate the enemy at the spawn point
        DamnedSoul enemy = Instantiate(enemyPrefab, spawnPoint + Vector3.up, Quaternion.identity).GetComponent<DamnedSoul>();
        // Set the enemy's parent to the room it is in
        enemy.transform.parent = transform;
        // Subscribe to the enemy's on died event so this script update the number of enemies killed
        enemy.onDied += OnEnemyDied;
    }

    private void OnEnemyDied()
    {
        // Increment the enemies killed counter
        enemiesKilled++;
        // Do nothing if the number of enemies killed is less than the number of enemies to kill in the wave
        if (enemiesKilled < enemiesToKill) { return; }

        // Increment the waves cleared counter
        wavesCleared++;
        // If there are still waves remaining, spawn another wave after the new wave delay
        if (wavesCleared < wavesToClear)
        {
            Invoke("SpawnWave", newWaveDelay);
        }
        // Otherwise, mark the room as cleared
        else
        {
            RoomCleared();
        }
    }

    private void RoomCleared()
    {
        // Set the room to cleared
        isRoomCleared = true;

        // Choose whether to spawn a heart using the heart spawn chance
        if (Random.value < heartSpawnChance)
        {
            GameObject healthPickup = Instantiate(healthPickupPrefab, transform.position + Vector3.up, Quaternion.identity);
            healthPickup.transform.parent = transform;
        }

        // Open the gates
        foreach (GameObject gate in gates)
        {
            if (gate.activeInHierarchy)
            {
                gate.GetComponent<Animator>().Play("Gate_Open");
            }
        }
    }
}
