using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangedImp : MonoBehaviour, IDamageable
{
    public float speed = 3f;
    public float attackRange = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private float nextFireTime;
    private int currentHealth;

    private ManaDropper manaDropper;

    [SerializeField] private int maxHealth = 20;
    private float curHealth;
    private bool isDead;

    public event Action onDied;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth; // Set health to full at start
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Maintain attack range while following the player
        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else if (distance < attackRange - 0.5f) // Move back slightly if too close
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
            // Signal that the enemy is dead
            onDied?.Invoke();
            // Drop mana
            //manaDropper.DropMana(transform.parent);
            // Destroy the enemy game object
            Destroy(gameObject);
        }
    }

    void Die()
    {
        Debug.Log("Imp has been defeated!");
        Destroy(gameObject); // Remove Imp from the game
    }

    //private void OnTriggerEnter(Collider other)
    //{
        // If Imp collides with specific tagged objects, take damage
        //if (other.CompareTag("Spell") || other.CompareTag("Dagger") || other.CompareTag("Chainsaw") || other.CompareTag("Bullet"))
        //{
            //TakeDamage(3); // Adjust damage as needed
        //}
    //}
}
