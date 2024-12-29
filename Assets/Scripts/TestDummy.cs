using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 20;
    private float curHealth;
    private bool isDead = false;

    private void Start()
    {
        // Initialise the dummy
        Initialise();
    }

    private void Initialise()
    {
        // Set the dummy's health to its max health
        curHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // Do nothing if the dummy is dead
        if (isDead) { return; }

        // Reduce health by the damage amount
        curHealth -= amount;

        // Kill the dummy if it reaches 0 health
        if (curHealth <= 0)
        {
            // Set the dummy to dead
            isDead = true;
            // Destroy the dummy object
            Destroy(gameObject);
        }
    }
}
