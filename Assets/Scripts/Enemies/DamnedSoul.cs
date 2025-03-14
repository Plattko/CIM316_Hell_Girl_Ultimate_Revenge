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

    // Movement/combat variables
    public float speed = 3f;
    public int damageAmount = 1;
    public float bounceBackDistance = 2f;
    private Vector3 bounceDirection;

    // Detection range
    public float detectionRange = 5f;

    // Health variables
    [SerializeField] private int maxHealth = 20;
    private float curHealth;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Get a reference to the mana dropper script
        manaDropper = GetComponentInChildren<ManaDropper>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Set the enemy's health to its max health
        curHealth = maxHealth;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * speed;
            }
            else
            {
                rb.velocity = Vector3.zero; // Stop movement if out of range
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Collsion Detected");
            // Damage the player via the ScriptableObject
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
                Debug.Log("Damage Delt");
            }

            // Bounce back
            bounceDirection = -(player.position - transform.position).normalized;
            StartCoroutine(BounceBack());
        }
    }

    IEnumerator BounceBack()
    {
        rb.velocity = bounceDirection * bounceBackDistance;
        yield return new WaitForSeconds(2f); // Short delay
        rb.velocity = Vector3.zero; // Stop movement after bounce
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
            manaDropper.DropMana(transform.parent);
            // Destroy the enemy game object
            Destroy(gameObject);
        }
    }
}