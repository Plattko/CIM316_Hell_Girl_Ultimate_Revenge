using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : MonoBehaviour
{
    [SerializeField] private GameObject[] gates = new GameObject[4];
    
    [HideInInspector] public bool isRoomCleared;
    private int maxWavesToClear = 2;
    private int wavesToClear;
    private int wavesCleared = 0;
    private float newWaveDelay = 2f;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemySpawnIndicatorPrefab;
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();
    private int minEnemyCount = 3;
    private int maxEnemyCount = 5;
    private int enemiesToKill;
    private int enemiesKilled = 0;

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
            // Randomly set the number to waves to clear between 1 and the max amount of waves to clear
            wavesToClear = Random.Range(1, maxWavesToClear + 1);
            // Spawn a wave
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        // Reset the enemies killed count
        enemiesKilled = 0;
        // Randomly set the number of enemies to kill to a number between the min and max enemy count
        enemiesToKill = Random.Range(minEnemyCount, maxEnemyCount + 1);
        // List of the chosen spawn points
        List<Transform> chosenSpawnPoints = new List<Transform>();

        // Randomly choose a number of spawn points equal to the number of enemies to kill without repetition
        for (int i = 0; i < enemiesToKill; i++)
        {
            Transform spawnPoint = null;
            // Randomly pick a spawn point from the list of enemy spawn points
            do
            {
                spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            }
            // Repeat if the spawn point has already been chosen
            while (chosenSpawnPoints.Contains(spawnPoint));
            // Add the spawn point to the list of chosen spawn points
            chosenSpawnPoints.Add(spawnPoint);
        }

        // Display an enemy spawn indicator and then spawn an enemy at each of the chosen spawn points
        foreach (Transform spawnPoint in chosenSpawnPoints)
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
        TestDummy enemy = Instantiate(enemyPrefab, spawnPoint + Vector3.up, Quaternion.identity).GetComponent<TestDummy>();
        // Set the enemy's parent to the room it is in
        enemy.transform.parent = transform;
        // TEMP: Disable respawning on the test dummy
        enemy.doesDummyRespawn = false;
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
