using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponAnimationEventHandler : MonoBehaviour
{
    public event Action onFinished;
    public event Action<bool> onAttackHitboxUpdated;

    public void AnimationFinishedTrigger() => onFinished?.Invoke();
    public void EnableAttackHitbox() => onAttackHitboxUpdated?.Invoke(true);
    public void DisableAttackHitbox() => onAttackHitboxUpdated?.Invoke(false);
}
