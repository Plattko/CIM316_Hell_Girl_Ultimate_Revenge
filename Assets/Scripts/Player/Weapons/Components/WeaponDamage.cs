using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : WeaponComponent<DamageData, AttackDamage>
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
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(curAttackData.Damage);
            }
        }
    }
}
