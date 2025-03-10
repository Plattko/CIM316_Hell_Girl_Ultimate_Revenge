using System.Collections;
using UnityEngine;
using System;

public class DamnedSoul : MonoBehaviour, IDamageable
{
    public float speed = 3f;
    public float bounceBackDistance = 2f;
    public int damageAmount = 1;
    public int health = 3;

    private Transform player;
    private Rigidbody rb;
    private Vector3 bounceDirection;

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

    // Reference to the player's ScriptableObject
    //public PlayerCharacter playerCharacter;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * speed;
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
        //else if (collision.CompareTag("Weapon") || collision.CompareTag("Spell"))
        //{
            //TakeDamage(1); // Take 1 damage when hit by weapon or spell
        //}
    }

    IEnumerator BounceBack()
    {
        rb.velocity = bounceDirection * bounceBackDistance;
        yield return new WaitForSeconds(2f); // Short delay
        rb.velocity = Vector3.zero; // Stop movement after bounce
    }

    public void TakeDamage(float amount)
    {
        // Do nothing if the dummy is dead
        if (isDead) { return; }
        Debug.Log("Testing");

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
            //col.enabled = false;
            // Hide it from view
            //meshRenderer.enabled = false;
            // Drop mana
            //DropMana();
            // Start the coroutine to respawn it after a delay if the dummy respawns
            Destroy(gameObject);
        }
    }

    //private void Die()
    //{
        //Destroy(gameObject); // Destroy the enemy
    //}
}