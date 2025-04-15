using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponAnimationEventHandler : MonoBehaviour
{
    public event Action OnFinished;
    public event Action OnStartMovement;
    public event Action OnStopMovement;
    public event Action OnAttackAction;

    public event Action<bool> onAttackHitboxUpdated;

    private void AnimationFinishedTrigger() => OnFinished?.Invoke();
    private void StartMovementTrigger() => OnStartMovement?.Invoke();
    private void StopMovementTrigger() => OnStopMovement?.Invoke();
    private void AttackActionTrigger() => OnAttackAction?.Invoke();

    public void EnableAttackHitbox() => onAttackHitboxUpdated?.Invoke(true);
    public void DisableAttackHitbox() => onAttackHitboxUpdated?.Invoke(false);
}
