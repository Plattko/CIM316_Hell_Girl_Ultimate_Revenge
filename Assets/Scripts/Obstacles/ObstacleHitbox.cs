using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHitbox : MonoBehaviour
{
    [SerializeField] private int playerDamage = 1;
    [SerializeField] private float enemyDamage = 10f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage(other, playerDamage);
        }
        else if (other.CompareTag("Enemy"))
        {
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
    }
}
