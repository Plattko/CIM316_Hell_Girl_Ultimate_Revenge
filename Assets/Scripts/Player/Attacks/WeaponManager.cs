using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    // Events
    public event Action<bool> onAttackStateChanged;

    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private GameObject tempHitbox;
    private Weapon weapon;

    private bool isAttacking;

    // TEMPORARY
    [SerializeField] private PlayerAim playerAim;

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
            SetAttackDirection();
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

    private void SetAttackDirection()
    {
        var (success, position) = playerAim.GetMouseWorldPosition();
        // Do nothing if getting the mouse's world position was unsuccessful
        if (!success) return;

        bool isAimingRight = position.x > transform.position.x;
        weaponSpriteRenderer.flipX = !isAimingRight;
        tempHitbox.transform.rotation = isAimingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
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
