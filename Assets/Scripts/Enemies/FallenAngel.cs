using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

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

    [Header("Beam Attack Settings")]
    public GameObject beamPrefab;
    public float beamDelay = 3f;

    [Header("Minion Summon Settings")]
    public GameObject[] minions;
    public Transform summonPoint;
    private bool summonedMinions = false;

    private Transform player;
    private bool usingBeamAttack = false;
    private Coroutine attackRoutine;

    private Unity.Mathematics.Random randomGenerator;

 

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Start projectile attack
        attackRoutine = StartCoroutine(FireProjectile());

        randomGenerator = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
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
            SwitchToBeamAttack();
        }
    }

    IEnumerator FireProjectile()
    {
        while (!usingBeamAttack)
        {
            if (player != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (player.position - firePoint.position).normalized;
                    rb.velocity = direction * projectileSpeed;
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
            int randomIndex = randomGenerator.NextInt(0, minions.Length);
            GameObject minion = minions[randomIndex];
            Instantiate(minion, summonPoint.position, Quaternion.identity);
        }
    }

    void SwitchToBeamAttack()
    {
        usingBeamAttack = true;
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        StartCoroutine(BeamAttack());
    }

    IEnumerator BeamAttack()
    {
        while (usingBeamAttack)
        {
            if (player != null)
            {
                Vector3 targetPosition = player.position;
                yield return new WaitForSeconds(0.5f); // Small delay before beam spawns
                Instantiate(beamPrefab, targetPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(beamDelay);
        }
    }

    public void TakeDamage(float amount)
    {
        // Do nothing if the enemy is dead
        if (isDead) { return; }

        // Reduce health by the damage amount
        currentHealth -= amount;

        // Kill the enemy if it reaches 0 health
        if (currentHealth <= 0)
        {
            // Set the enemy to dead
            isDead = true;
            // Signal that the enemy died
            onDied?.Invoke();
            // Drop mana
            //manaDropper.DropMana(transform.parent);
            // Destroy the enemy game object
            Destroy(gameObject);
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
