using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    // Events
    public event Action<bool> onAttackStateChanged;
    
    private Weapon weapon;

    private bool isAttacking;

    private void Awake()
    {
        weapon = transform.Find("Weapon").GetComponent<Weapon>();
        weapon.onExit += ExitHandler;
    }

    public void StartAttacking()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            onAttackStateChanged?.Invoke(true);
            weapon.Enter();
        }
    }

    public void StopAttacking()
    {
        
    }

    private void ExitHandler()
    {
        isAttacking = false;
        onAttackStateChanged?.Invoke(false);
    }


    //-------------------------------------------------------------
    // WEAPON MANAGEMENT
    //-------------------------------------------------------------
    //public void SwapWeapon(WeaponSO newWeapon)
    //{
    //    // Set the player's current weapon to the new weapon
    //    curWeaponSO = newWeapon;
    //}
}
