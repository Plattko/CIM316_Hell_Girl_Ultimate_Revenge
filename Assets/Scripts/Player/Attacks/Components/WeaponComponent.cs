using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponComponent : MonoBehaviour
{
    protected Weapon weapon;
    //protected WeaponAnimationEventHandler EventHandler => weapon.AnimEventHandler;
    protected WeaponAnimationEventHandler eventHandler;
    protected PlayerController PlayerController => weapon.PlayerController;

    protected bool isAttackActive;

    protected virtual void Awake()
    {
        weapon = GetComponent<Weapon>();

        eventHandler = GetComponentInChildren<WeaponAnimationEventHandler>();
    }

    protected virtual void OnEnable()
    {
        weapon.onEnter += HandleEnter;
        weapon.onExit += HandleExit;
    }

    protected virtual void OnDisable()
    {
        weapon.onEnter -= HandleEnter;
        weapon.onExit -= HandleExit;
    }

    protected virtual void HandleEnter()
    {
        isAttackActive = true;
    }

    protected virtual void HandleExit()
    {
        isAttackActive = false;
    }
}
