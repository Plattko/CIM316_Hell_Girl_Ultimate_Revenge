using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHitbox : MonoBehaviour
{
    [SerializeField] private int playerDamage = 1;
    [SerializeField] private float enemyDamage = 10f;
    [SerializeField] private bool dealKnockback;
    [SerializeField] private float knockbackStrength = 5f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage(other, playerDamage);
        }
        else if (other.CompareTag("Enemy"))
        {
            // Do nothing if it is a divine enemy
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sharedMaterial == GameManager.Instance.DivineEnemyMaterial) return;

            DealDamage(other, enemyDamage);
        }
    }

    private void DealDamage(Collider other, float damage)
    {
        // Deal damage if it hit a damageable object
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Deal damage
            damageable.TakeDamage(damage);
        }
        // Apply knockback if object is knockbackable
        if (dealKnockback && other.TryGetComponent(out IKnockbackable knockbackable))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            direction.y = 0;
            knockbackable.Knockback(direction, knockbackStrength);
        }
    }
}
