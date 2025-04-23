using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossRoom : MonoBehaviour
{
    // Gate references
    [SerializeField] private GameObject[] gates = new GameObject[4];

    [field: SerializeField] public Transform InitialEntryPoint { get; private set; }

    // Clear variables
    [SerializeField] private FallenAngel miniboss;
    private List<GameObject> additionalEnemies = new List<GameObject>();
    public bool IsRoomCleared { get; private set; }

    // Health pickup variables
    [SerializeField] private GameObject healthPickupPrefab;
    private float heartSpawnChance = 0.33f;

    private void OnEnable()
    {
        miniboss.onDied += RoomCleared;
        miniboss.onMinionSummoned += OnMinionSummoned;
    }

    private void OnDisable()
    {
        miniboss.onDied -= RoomCleared;
        miniboss.onMinionSummoned -= OnMinionSummoned;
    }

    public void InitialiseRoom()
    {
        if (IsRoomCleared) return;
        
        // Close the gates to prevent the player from leaving
        foreach (GameObject gate in gates)
        {
            if (gate.activeInHierarchy)
            {
                gate.GetComponent<Animator>().Play("Gate_Close");
            }
        }
    }

    private void OnMinionSummoned(GameObject minion)
    {
        additionalEnemies.Add(minion);
    }

    private void RoomCleared()
    {
        // Set the room to cleared
        IsRoomCleared = true;

        // Choose whether to spawn a heart using the heart spawn chance
        if (Random.value < heartSpawnChance)
        {
            GameObject healthPickup = Instantiate(healthPickupPrefab, transform.position + new Vector3(0, 1, -2), Quaternion.identity);
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

        // Kill the additional enemies
        foreach (GameObject enemy in additionalEnemies)
        {
            if (enemy == null) continue;

            //if (enemy.TryGetComponent(out IDamageable damageable))
            //{
            //    damageable.TakeDamage(float.MaxValue);
            //}

            // Destroy the enemy game object
            Destroy(enemy);
        }

        // Make the ascension leech descend
        AscensionLeech leech = GetComponentInChildren<AscensionLeech>();
        leech.StartCoroutine(leech.Descend());

        // Increase the rooms cleared stat
        StatsManager.Instance.IncreaseRoomsCleared();
    }
}
