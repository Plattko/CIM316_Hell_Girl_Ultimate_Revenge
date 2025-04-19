using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnockback : WeaponComponent<KnockbackData, AttackKnockback>
{
    private WeaponActionHitbox hitbox;

    protected override void Start()
    {
        base.Start();

        hitbox = GetComponent<WeaponActionHitbox>();

        hitbox.OnDetectedCollider += HandleDetectedCollider;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        hitbox.OnDetectedCollider -= HandleDetectedCollider;
    }

    private void HandleDetectedCollider(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IKnockbackable knockbackable))
            {
                Vector3 direction = (collider.transform.position - PlayerController.transform.position).normalized;
                knockbackable.Knockback(direction, curAttackData.Strength);
            }
        }
    }
}
