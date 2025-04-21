using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    // Gate references
    [SerializeField] private GameObject[] gates = new GameObject[4];

    [field: SerializeField] public Transform InitialEntryPoint { get; private set; }

    // Clear variables
    public bool IsRoomCleared { get; private set; }

    // Health pickup variables
    [SerializeField] private GameObject healthPickupPrefab;
    private float heartSpawnChance = 0.33f;

    // End of demo variables
    private float endDemoDelay = 0.5f;

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

        StartCoroutine(EndDemo());
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

        // Make the ascension leech descend
        AscensionLeech leech = GetComponentInChildren<AscensionLeech>();
        leech.StartCoroutine(leech.Descend());
    }

    private IEnumerator EndDemo()
    {
        yield return new WaitForSeconds(endDemoDelay);
        GameManager.Instance.EndDemo();
    }
}
