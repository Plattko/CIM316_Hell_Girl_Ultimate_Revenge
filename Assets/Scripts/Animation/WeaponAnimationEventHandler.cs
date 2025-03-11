using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponAnimationEventHandler : MonoBehaviour
{
    public event Action onFinished;

    public void AnimationFinishedTrigger() => onFinished?.Invoke();
}
