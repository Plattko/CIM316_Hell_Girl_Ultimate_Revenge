using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponActionHitbox : WeaponComponent<ActionHitboxData, AttackActionHitbox>
{
    public event Action<Collider[]> OnDetectedCollider;
    
    private Vector3 offset;

    private Collider[] detected;
    
    protected override void Start()
    {
        base.Start();

        eventHandler.OnAttackAction += HandleAttackAction;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        eventHandler.OnAttackAction -= HandleAttackAction;
    }

    private void HandleAttackAction()
    {
        int offsetDir = PlayerController.isFacingRight ? 1 : -1;
        offset.Set(
            transform.position.x + (curAttackData.HitboxOffset.x * offsetDir),
            transform.position.y + curAttackData.HitboxOffset.y,
            transform.position.z + curAttackData.HitboxOffset.z
            );


        detected = Physics.OverlapSphere(offset, curAttackData.HitboxRadius, data.DetectableLayers);

        if (detected.Length == 0) return;
        OnDetectedCollider?.Invoke(detected);
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        foreach (AttackActionHitbox item in data.AttackData)
        {
            if (!item.Debug) continue;
            Gizmos.DrawWireSphere(transform.position + item.HitboxOffset, item.HitboxRadius);
        }
    }
}
