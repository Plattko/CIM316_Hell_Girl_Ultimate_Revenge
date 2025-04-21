using System.Collections;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

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

    //Animation variables

    private Animator animator;
    public bool IsMoving { get; private set; }
    public bool IsAttacking { get; private set; }



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        manaDropper = GetComponentInChildren<ManaDropper>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        curHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        if (isEngaged)
        {
            if (!isBouncing)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * speed;
            }
        }
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
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Collision detected.");
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                StartCoroutine(HandleAttack(damageable));
            }

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (!playerController.isDashing)
            {
                StartCoroutine(BounceBack());
            }
        }
    }

    private IEnumerator HandleAttack(IDamageable damageable)
    {
        // Calculate direction to player first
        Vector3 directionToPlayer = player.position - transform.position;

        // Flip sprite to face the player (assuming default face-left sprite)
        if (directionToPlayer.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

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

    IEnumerator BounceBack()
    {
        // Slight delay before bounce starts
        yield return new WaitForSeconds(0.2f); // Adjust as needed for timing

        isBouncing = true;

        Vector3 bounceDirection = -(player.position - transform.position).normalized;
        rb.velocity = bounceDirection * (bounceBackDistance / bounceBackDuration);

        yield return new WaitForSeconds(bounceBackDuration);

        rb.velocity = Vector3.zero;
        isBouncing = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        curHealth -= amount;

        if (curHealth <= 0)
        {
            isDead = true;
            onDied?.Invoke();
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        // Set the IsDead flag to true to trigger the death animation
        animator.SetBool("IsDead", true);

        // Stop all movement immediately
        rb.velocity = Vector3.zero;

        // Drop mana
        manaDropper.DropMana(transform.parent);

        // Ensure the death animation is playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Wait until the death animation finishes playing
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        // Destroy the object after the death animation finishes
        Destroy(gameObject);
    }
}