using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FallenAngel : MonoBehaviour, IDamageable
{
    // Events
    public event Action onDied;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 1.5f;
    private int attackCount = 0;

    [Header("Beam Attack Settings")]
    public GameObject beamPrefab;
    public float beamDelay = 3f;
    public int beamsPerAttack = 5; // Number of beams per beam attack

    [Header("Minion Summon Settings")]
    public GameObject[] minions;
    public Transform summonPoint;
    private bool summonedMinions = false;

    private Transform player;
    private bool usingBeamAttack = false;
    private Coroutine attackRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Start projectile attack loop
        attackRoutine = StartCoroutine(FireProjectile());
    }

    void Update()
    {
        if (player == null) return;

        // Health-based behavior changes
        if (currentHealth <= maxHealth * 0.75f && !summonedMinions)
        {
            SummonMinions();
        }

        if (currentHealth <= maxHealth * 0.5f && !usingBeamAttack)
        {
            usingBeamAttack = true; // Enable beam attacks at 50% health
        }
    }

    IEnumerator FireProjectile()
    {
        while (!isDead)
        {
            if (player != null)
            {
                // Fire projectile
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                ImpProjectile projScript = projectile.GetComponent<ImpProjectile>();
                if (projScript != null)
                {
                    projScript.SetDirection(player.position - firePoint.position);
                }

                attackCount++;

                // If below 50% health, trigger beam attack every 5 ranged attacks
                if (usingBeamAttack && attackCount % 5 == 0)
                {
                    yield return StartCoroutine(BeamAttack());
                }
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    void SummonMinions()
    {
        summonedMinions = true;
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, minions.Length);
            GameObject minion = minions[randomIndex];
            Instantiate(minion, summonPoint.position, Quaternion.identity);
        }
    }

    IEnumerator BeamAttack()
    {
        yield return new WaitForSeconds(0.5f); // Small delay before beams spawn

        for (int i = 0; i < beamsPerAttack; i++)
        {
            SpawnBeamAtRandomGround();
        }

        yield return new WaitForSeconds(beamDelay);
    }

    void SpawnBeamAtRandomGround()
    {
        // Get all active ground objects
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground")
            .Where(obj => obj.activeInHierarchy)
            .ToArray();

        if (groundObjects.Length == 0)
        {
            Debug.LogWarning("No active ground objects found!");
            return;
        }

        // Pick a random ground object
        GameObject randomGround = groundObjects[UnityEngine.Random.Range(0, groundObjects.Length)];
        Collider groundCollider = randomGround.GetComponent<Collider>();

        if (groundCollider == null)
        {
            Debug.LogWarning("Selected ground object has no Collider!");
            return;
        }

        // Get a random position within the bounds
        Vector3 spawnPosition = GetRandomPointInBounds(groundCollider.bounds);

        // Spawn the beam
        Instantiate(beamPrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y; // Ensure the beam spawns on top

        return new Vector3(x, y, z);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            isDead = true;
            onDied?.Invoke();
            Destroy(gameObject);
        }
    }
}
