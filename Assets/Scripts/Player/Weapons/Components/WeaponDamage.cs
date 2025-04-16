using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : WeaponComponent<DamageData, AttackDamage>
{
    private WeaponActionHitbox hitbox;

    protected override void Awake()
    {
        base.Awake();

        hitbox = GetComponent<WeaponActionHitbox>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        hitbox.OnDetectedCollider += HandleDetectedCollider;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

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
