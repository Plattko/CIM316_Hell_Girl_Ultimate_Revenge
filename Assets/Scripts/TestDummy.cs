using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : MonoBehaviour
{
    [SerializeField] private Damageable damageable;
    [SerializeField] private int maxHealth = 20;
    private float curHealth;

    private void Start()
    {
        curHealth = maxHealth;
    }

    private void OnEnable()
    {
        damageable.onDamaged += TakeDamage;
    }

    private void OnDisable()
    {
        damageable.onDamaged -= TakeDamage;
    }

    private void TakeDamage(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
