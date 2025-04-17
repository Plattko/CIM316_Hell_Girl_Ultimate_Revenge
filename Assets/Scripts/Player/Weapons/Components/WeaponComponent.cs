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

    protected virtual void Start()
    {
        weapon.onEnter += HandleEnter;
        weapon.onExit += HandleExit;
    }

    protected virtual void OnDestroy()
    {
        weapon.onEnter -= HandleEnter;
        weapon.onExit -= HandleExit;
    }

    public virtual void Initialise() { }

    protected virtual void HandleEnter()
    {
        isAttackActive = true;
    }

    protected virtual void HandleExit()
    {
        isAttackActive = false;
    }
}

public abstract class WeaponComponent<T1, T2> : WeaponComponent where T1 : ComponentData<T2> where T2 : AttackData
{
    protected T1 data;
    protected T2 curAttackData;

    public override void Initialise()
    {
        base.Initialise();

        data = weapon.Data.GetData<T1>();
    }

    protected override void HandleEnter()
    {
        base.HandleEnter();

        curAttackData = data.AttackData[weapon.CurAttackCounter];
    }
}
