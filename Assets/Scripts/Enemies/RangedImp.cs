using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangedImp : MonoBehaviour, IDamageable
{
    // Events
    public event Action onDied;

    // Reference variables
    private Transform player;
    private ManaDropper manaDropper;

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;
    private float curHealth;
    private bool isDead;

    [Header("Movement & Combat")]
    public float speed = 3f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 5f;
    public float fireRate = 1f;
    public float initialFireDelay = 1f;
    private float nextFireTime;

    void Start()
    {
        // Get a reference to the mana dropper script
        manaDropper = GetComponentInChildren<ManaDropper>();
        // Get a reference to the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        // Set the enemy's health to its max health
        curHealth = maxHealth;
        // Set the enemy's nextFireTime to the initial fire delay so it doesn't shoot immediately upon spawning
        nextFireTime = Time.time + initialFireDelay;
    }

    void Update()
    {
        // Do nothing if the player is null
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Maintain attack range while following the player
        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        else if (distanceToPlayer < attackRange - 0.5f) // Move back slightly if too close
        {
            MoveAwayFromPlayer();
        }

        // Shoot projectiles while keeping attack range
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void MoveAwayFromPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ImpProjectile projScript = projectile.GetComponent<ImpProjectile>();

        if (projScript != null)
        {
            projScript.SetDirection(player.position - firePoint.position);
        }
    }

    public void TakeDamage(float amount)
    {
        // Do nothing if the enemy is dead
        if (isDead) { return; }

        // Reduce health by the damage amount
        curHealth -= amount;

        // Kill the enemy if it reaches 0 health
        if (curHealth <= 0)
        {
            // Set the enemy to dead
            isDead = true;
            // Signal that the enemy died
            onDied?.Invoke();
            // Drop mana
            manaDropper.DropMana(transform.parent);
            // Destroy the enemy game object
            Destroy(gameObject);
        }
    }
}
