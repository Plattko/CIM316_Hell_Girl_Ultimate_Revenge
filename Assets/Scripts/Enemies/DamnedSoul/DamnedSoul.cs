using System.Collections;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class DamnedSoul : MonoBehaviour, IDamageable, IKnockbackable
{
    // Events
    public event Action onDied;

    // Reference variables
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private CapsuleCollider triggerCollider;
    [SerializeField] private CapsuleCollider environmentCollider;
    private Transform player;
    private Rigidbody rb;
    private DamageFlash damageFlash;
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

    private Coroutine attackCoroutine;

    // Knockback
    private bool isInKnockback;
    private float knockbackDuration = 0.5f;
    private Coroutine knockbackCoroutine;

    //Animation variables

    private Animator animator;
    public bool IsMoving { get; private set; }
    public bool IsAttacking { get; private set; }

    [Header("UI")]
    public Slider healthSlider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Get a reference to the animator
        animator = GetComponent<Animator>();
        // Get a reference to the damage flash script
        damageFlash = GetComponent<DamageFlash>();
        // Get a reference to the mana dropper script
        manaDropper = GetComponentInChildren<ManaDropper>();
        // Get a reference to the player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Set the enemy's health to its max health
        curHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = curHealth;
        }
    }

    void Update()
    {
        // Do nothing if the player is null
        if (player == null || isDead) return;

        // Calculate direction to player first
        Vector3 directionToPlayer = player.position - transform.position;
        // Flip sprite to face the player (assuming default face-left sprite)
        sr.flipX = directionToPlayer.x > 0 ? true : false;

        // Check if the enemy is engaged
        if (isEngaged)
        {
            // Chase the player if the enemy isn't bouncing back or in knockback
            if (!isBouncing && !isInKnockback)
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

        // Check if the Rigidbody is moving
        IsMoving = rb.velocity.magnitude > 0.1f;
        animator.SetBool("IsMoving", IsMoving);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Do nothing if in knockback or dead
        if (isInKnockback || isDead) return;
        // Do nothing if it isn't the player
        if (!collision.CompareTag("Player")) return;
        // Do nothing if the player is dashing
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController.isDashing) return;

        // Attack the player and bounce back
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            attackCoroutine = StartCoroutine(HandleAttack(damageable));
            StartCoroutine(BounceBack());
        }
    }

    private IEnumerator HandleAttack(IDamageable damageable)
    {
        // Trigger attack animation
        IsAttacking = true;
        animator.SetBool("IsAttacking", true);

        // Delay for wind-up
        yield return new WaitForSeconds(0.3f); // adjust this to match your animation timing

        // Apply damage
        damageable.TakeDamage(damageAmount);
        Debug.Log("Damage dealt.");

        // Wait for animation to finish before resetting
        yield return new WaitForSeconds(0.5f); // adjust to match your attack anim length

        IsAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    private void InterruptAttack()
    {
        if (attackCoroutine == null) return;

        StopCoroutine(attackCoroutine);
        IsAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    private IEnumerator BounceBack()
    {
        // Slight delay before bounce starts
        yield return new WaitForSeconds(0.2f); // Adjust as needed for timing

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
        if (isDead) return;

        // Interrupt the enemy's attack
        InterruptAttack();
        // Reduce health by the damage amount
        curHealth -= amount;
        // Play the damage flash
        damageFlash.DoDamageFlash();
        //sets health slider to current health
        if (healthSlider != null)
        {
            healthSlider.value = curHealth;
        }

        // Kill the enemy if it reaches 0 health
        if (curHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Set the enemy to dead
        isDead = true;
        // Signal that the enemy died
        onDied?.Invoke();
        // Set the IsDead flag to true to trigger the death animation
        animator.SetBool("IsDead", true);

        // Set its rigidbody to kinematic so it doesn't move
        rb.isKinematic = true;
        // Disable the colliders
        triggerCollider.enabled = false;
        environmentCollider.enabled = false;

        // Drop mana
        if (manaDropper != null)
        {
            manaDropper.DropMana(transform.parent);
        }

        if (!GameManager.Instance.IsInOnboarding)
        {
            // Increase the enemies killed stat
            StatsManager.Instance.IncreaseEnemiesKilled();
        }
    }

    public void Destroy()
    {
        // Destroy the object after the death animation finishes
        Destroy(gameObject);
    }

    //-------------------------------------------------------------
    // KNOCKBACK
    //-------------------------------------------------------------
    public void Knockback(Vector3 direction, float strength)
    {
        // Do nothing if the enemy is dead
        if (isDead) return;

        isInKnockback = true;
        InterruptKnockback();
        knockbackCoroutine = StartCoroutine(TakeKnockback(direction * strength));
    }

    private IEnumerator TakeKnockback(Vector3 initVel)
    {
        float elapsedTime = 0;

        while (elapsedTime < knockbackDuration)
        {
            rb.velocity = Vector3.Lerp(initVel, Vector3.zero, elapsedTime / knockbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isInKnockback = false;
    }

    private void InterruptKnockback()
    {
        if (knockbackCoroutine == null) return;
        StopCoroutine(knockbackCoroutine);
        isInKnockback = false;
    }
}