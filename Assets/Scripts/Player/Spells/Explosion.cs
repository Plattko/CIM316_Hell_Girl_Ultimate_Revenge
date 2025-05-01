using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float duration;
    private float knockbackStrength = 12f;

    [Header("SFX")]
    [SerializeField] private AudioClip explosionSFX;

    public void Initialise(float _damage, float _duration)
    {
        damage = _damage;
        duration = _duration;

        // Play the explosion SFX
        SFXManager.Instance.PlayAudioClip(explosionSFX, transform, 0.5f, 1f, true);

        // Destroy the explosion after the duration ends
        Destroy(gameObject, duration);
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
    }
}
