using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    private int isMovePressedHash;
    private int moveSpeedHash;
    private int isDashingHash;
    private int isAttackingHash;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        // Store the animator's string parameters as integers to use a simpler data type and improve performance
        isMovePressedHash = Animator.StringToHash("isMovePressed");
        moveSpeedHash = Animator.StringToHash("moveSpeed");
        isDashingHash = Animator.StringToHash("isDashing");
        isAttackingHash = Animator.StringToHash("isAttacking");
    }

    public void SetIsMovePressed(bool _isMovePressed)
    {
        animator.SetBool(isMovePressedHash, _isMovePressed);
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        animator.SetFloat(moveSpeedHash, moveSpeed);
    }

    public void SetIsDashing(bool isDashing)
    {
        animator.SetBool(isDashingHash, isDashing);
    }

    public void SetIsAttacking(bool isAttacking)
    {
        animator.SetBool(isAttackingHash, isAttacking);
    }
}
