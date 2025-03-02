using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 20;
    private float curHealth;
    private bool isDead = false;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider col;
    private Vector3 startingPos;

    public bool doesDummyRespawn = true;
    [SerializeField] private float respawnDelay = 5.0f;
    public event Action onDied;

    [Header("Mana Drop Variables")]
    [SerializeField] private GameObject manaPickupPrefab;
    [SerializeField] private int minManaDrop = 1;
    [SerializeField] private int maxManaDrop = 3;
    [SerializeField] private float minDropForceX = 2;
    [SerializeField] private float maxDropForceX = 3;
    [SerializeField] private float dropForceY = 2;

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        // Set the dummy's health to its max health
        curHealth = maxHealth;
        // Set the dummy's starting position to its current position
        startingPos = transform.position;
    }

    public void TakeDamage(float amount)
    {
        // Do nothing if the dummy is dead
        if (isDead) { return; }

        // Reduce health by the damage amount
        curHealth -= amount;

        // Kill the dummy if it reaches 0 health
        if (curHealth <= 0)
        {
            // Set the dummy to dead
            isDead = true;
            // Signal that the dummy is dead
            onDied?.Invoke();
            // Disable its collider
            col.enabled = false;
            // Hide it from view
            meshRenderer.enabled = false;
            // Drop mana
            DropMana();
            // Start the coroutine to respawn it after a delay if the dummy respawns
            if (doesDummyRespawn)
            {
                StartCoroutine(RespawnAfterDelay());
            }
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        // Wait for the duration of the respawn delay and then respawn the dummy
        yield return new WaitForSecondsRealtime(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        // Set the dummy's health back to its max health
        curHealth = maxHealth;
        // Set its position back to the starting position
        transform.position = startingPos;
        // Re-enable its collider
        col.enabled = true;
        // Un-hide it from view
        meshRenderer.enabled = true;
        // Set it back to alive
        isDead = false;
    }

    private void DropMana()
    {
        // Roll the amount of mana to drop
        int manaDropAmount = UnityEngine.Random.Range(minManaDrop, maxManaDrop);
        // Spawn a number of mana pickups equal to the mana drop amount and give each one a random drop force
        for (int i = 0; i < manaDropAmount; i++)
        {
            // Instantiate the mana pickup
            GameObject manaPickup = Instantiate(manaPickupPrefab, transform.position, Quaternion.identity);
            // Set its parent to the parent of the dummy (e.g. the room the dummy is in)
            manaPickup.transform.parent = transform.parent;
            // Calculate the horizontal drop force
            Vector2 dropForceX = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(minDropForceX, maxDropForceX);
            // Calculate the total drop force
            Vector3 dropForce = new Vector3(dropForceX.x, dropForceY, dropForceX.y);
            Debug.Log("Drop force: " + dropForce);
            // Apply the drop force to the mana pickup so it's thrown slightly in a random direction
            manaPickup.GetComponent<Rigidbody>().AddForce(dropForce, ForceMode.Impulse);
        }
    }
}
