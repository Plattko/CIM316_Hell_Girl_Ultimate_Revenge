using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpProjectile : MonoBehaviour

{
    private Rigidbody rb;

    public float speed = 10f;
    public float lifetime = 3f;
    private Vector3 direction;
    public int damageAmount = 1;

    [Header("VFX")]
    [SerializeField] private GameObject impactVFX;

    [Header("SFX")]
    [SerializeField] private AudioClip impactSFX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(rb);
        Destroy(gameObject, lifetime); // Destroy after time
    }

    public void Initialise(Vector3 dir)
    {
        rb = GetComponent<Rigidbody>();
        dir.y = 0;
        direction = dir.normalized;
        rb.velocity = direction * speed;
        Debug.Log(rb.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Damage the player via the ScriptableObject
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
            }
            // Destroy the projectile if the player isn't dashing
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (!playerController.isDashing)
            {
                // Play the impact SFX
                SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1.25f, true);
                SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1.25f, true);
                SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1.25f, true);

                Destroy(gameObject);
            }
        }
        // Destroy the projectile if it collides with something other than an enemy or pickup
        else if (!collision.CompareTag("Enemy") && !collision.CompareTag("Pickup"))
        {
            // Play the impact SFX
            SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1.25f, true);
            SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1.25f, true);
            SFXManager.Instance.PlayAudioClip(impactSFX, transform, 1f, 1.25f, true);

            // Spawn the impact VFX
            Instantiate(impactVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
