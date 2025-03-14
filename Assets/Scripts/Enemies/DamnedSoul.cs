using System.Collections;
using UnityEngine;
using System;

public class DamnedSoul : MonoBehaviour, IDamageable
{
    // Events
    public event Action onDied;

    // Reference variables
    private Transform player;
    private Rigidbody rb;
    private ManaDropper manaDropper;

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;
    private float curHealth;
    private bool isDead;

    [Header("Movement & Combat")]
    public float speed = 3f;
    public int damageAmount = 1;

    public float bounceBackDistance = 2f;
    public float bounceBackDuration = 1f;
    private bool isBouncing;

    public float detectionRange = 5f;
    private bool isEngaged;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Get a reference to the mana dropper script
        manaDropper = GetComponentInChildren<ManaDropper>();
        // Get a reference to the player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Set the enemy's health to its max health
        curHealth = maxHealth;
    }

    void Update()
    {
        // Do nothing if the player is null
        if (player == null) return;

        // Check if the enemy is engaged
        if (isEngaged)
        {
            // Chase the player if the enemy isn't bouncing back
            if (!isBouncing)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * speed;
            }
        }
        // If they aren't engaged, check the distance to the player and become engaged if they are in range
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                isEngaged = true;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Collsion detected.");
            // Damage the player
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
                Debug.Log("Damage dealt.");
            }

            // Bounce back if the player isn't dashing
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (!playerController.isDashing)
            {
                StartCoroutine(BounceBack());
            }
        }
    }

    IEnumerator BounceBack()
    {
        isBouncing = true;
        // Set the bounce direction to the opposite of the direction to the player
        Vector3 bounceDirection = -(player.position - transform.position).normalized;
        // Set the enemy's velocity to the speed required to travel the bounce back distance over its duration in the bounce back direction
        rb.velocity = bounceDirection * (bounceBackDistance / bounceBackDuration);
        // Wait for the bounce back duration
        yield return new WaitForSeconds(bounceBackDuration);
        // Stop movement after bounce
        rb.velocity = Vector3.zero;
        isBouncing = false;
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