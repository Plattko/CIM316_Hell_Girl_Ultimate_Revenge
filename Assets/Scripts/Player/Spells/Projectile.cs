using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private float moveSpeed;
    private float damage;
    private float lifetime;
    private float knockbackStrength = 4f;

    [Header("VFX")]
    [SerializeField] private GameObject impactVFX;

    [Header("SFX")]
    [SerializeField] private AudioClip thrownSFX;
    [SerializeField] private AudioClip impactSFX;

    public void Initialise(Vector3 moveDir, float _moveSpeed, float _damage, float _duration)
    {
        moveSpeed = _moveSpeed;
        damage = _damage;
        lifetime = _duration;

        // Play the thrown SFX
        SFXManager.Instance.PlayAudioClip(thrownSFX, transform, 1f, 1f, true);

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
        // Apply knockback if object is knockbackable
        if (other.TryGetComponent(out IKnockbackable knockbackable))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            direction.y = 0;
            knockbackable.Knockback(direction, knockbackStrength);
        }

        // Play the impact SFX
        SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1f, true);
        SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1f, true);
        SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1f, true);
        // Spawn the impact VFX
        Instantiate(impactVFX, transform.position, Quaternion.identity);
        // Destroy the projectile
        Destroy(gameObject);
    }
}
