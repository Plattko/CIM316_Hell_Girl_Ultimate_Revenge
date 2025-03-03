using System.Collections;
using UnityEngine;

public class DamnedSoul : MonoBehaviour
{
    public float speed = 3f;
    public float bounceBackDistance = 2f;
    public int damageAmount = 1;
    public int health = 3;

    private Transform player;
    private Rigidbody rb;
    private Vector3 bounceDirection;

    // Reference to the player's ScriptableObject
    public PlayerCharacter playerCharacter;

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
            if (playerCharacter != null)
            {
                playerCharacter.TakeDamage(damageAmount);
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
        yield return new WaitForSeconds(0.2f); // Short delay
        rb.velocity = Vector3.zero; // Stop movement after bounce
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject); // Destroy the enemy
    }
}