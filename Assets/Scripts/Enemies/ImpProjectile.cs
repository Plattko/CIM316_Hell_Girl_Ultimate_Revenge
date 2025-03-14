using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpProjectile : MonoBehaviour

{
    public float speed = 10f;
    public float lifetime = 3f;
    private Vector3 direction;
    public int damageAmount = 1;


    void Start()
    {
         Destroy(gameObject, lifetime); // Destroy after time
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Collsion Detected");
            // Damage the player via the ScriptableObject
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
                Debug.Log("Damage Delt");
            }
        }
    }
}
