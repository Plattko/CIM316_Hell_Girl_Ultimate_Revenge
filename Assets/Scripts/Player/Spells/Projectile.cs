using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float damage;
    [HideInInspector] public float lifetime;

    public void Initialise(Vector3 moveDir, float _moveSpeed, float _damage, float _duration)
    {
        moveSpeed = _moveSpeed;
        damage = _damage;
        lifetime = _duration;

        // Set the project's velocity
        rb.velocity = moveDir * moveSpeed;

        // Destroy the projectile at the end of its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deal damage if it hit a damageable object
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Deal damage
            damageable.TakeDamage(damage);
            // Print the damage dealt
            Debug.Log("Damaged " + other.name + " for " + damage + " damage.");
        }

        // Destroy the projectile
        Destroy(gameObject);
    }
}
