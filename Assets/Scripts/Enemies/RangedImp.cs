using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class RangedImp : MonoBehaviour, IDamageable
{
    // Events
    public event Action onDied;

    // Reference variables
    [SerializeField] private SpriteRenderer sr;
    private Transform player;
    private DamageFlash damageFlash;
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
    private Animator animator;

    private Vector3 lastPosition;
    private float movementThreshold = 0.01f;

    [Header("UI")]
    public Slider healthSlider;

    [Header("SFX")]
    [SerializeField] private AudioClip[] damageSFX;
    [SerializeField] private AudioClip[] deathSFX;

    void Start()
    {
        lastPosition = transform.position;
        // Get a reference to the damage flash script
        damageFlash = GetComponent<DamageFlash>();
        // Get a reference to the mana dropper script
        manaDropper = GetComponentInChildren<ManaDropper>();
        // Get a reference to the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        // Set the enemy's health to its max health
        curHealth = maxHealth;
        // Set the enemy's nextFireTime to the initial fire delay so it doesn't shoot immediately upon spawning
        nextFireTime = Time.time + initialFireDelay;
        animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = curHealth;
        }
    }

    void Update()
    {
        Vector3 movement = transform.position - lastPosition;

        if (animator != null)
        {
            bool isMoving = movement.magnitude > movementThreshold;
            animator.SetBool("IsMoving", isMoving);
        }

        lastPosition = transform.position;
        // Do nothing if the player is null
        if (player == null) return;

        if (player != null)
        {
            // Flip sprite to face player
            Vector3 directionToPlayer = player.position - transform.position;
            sr.flipX = directionToPlayer.x > 0 ? true : false;
        }

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
        StartCoroutine(ShootWithAnimationDelay());
    }

    public void TakeDamage(float amount)
    {
        // Do nothing if the enemy is dead
        if (isDead) { return; }

        // Reduce health by the damage amount
        curHealth -= amount;
        // Play the damage flash
        damageFlash.DoDamageFlash();

        if (healthSlider != null)
        {
            healthSlider.value = curHealth;
        }

        if (curHealth > 0)
        {
            // Play the damage SFX
            SFXManager.Instance.PlayRandomAudioClip(damageSFX, transform, 0.5f, 1f, true);
        }
        // Kill the enemy if it reaches 0 health
        else
        {
            // Set the enemy to dead
            isDead = true;
            // Signal that the enemy died
            onDied?.Invoke();
            // Play the death SFX
            SFXManager.Instance.PlayRandomAudioClip(deathSFX, transform, 0.5f, 1f, true);
            // Drop mana
            manaDropper.DropMana(transform.parent);

            // Increase the enemies killed stat
            StatsManager.Instance.IncreaseEnemiesKilled();

            // Destroy the enemy game object
            Destroy(gameObject);
        }
    }

    IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.2f); // Adjust timing to match animation
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    IEnumerator ShootWithAnimationDelay()
    {
        // Trigger the attack animation
        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
        }

        // Wait a moment before firing to sync with the animation
        yield return new WaitForSeconds(0.2f); // adjust to fit your animation

        // Fire the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ImpProjectile projScript = projectile.GetComponent<ImpProjectile>();

        if (projScript != null)
        {
            projScript.Initialise(player.position - firePoint.position);
        }

        // Reset attack animation after it's played
        yield return new WaitForSeconds(0.2f); // adjust based on anim length
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }
    }
}
