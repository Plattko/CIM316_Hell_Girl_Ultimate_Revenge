using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;

    private Vector3 moveInput;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float idleSlow = 0.9f;

    [HideInInspector] public Vector2 lastMoveDir;
    private bool isFacingRight = true;

    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;

    void Start()
    {
        
    }

    
    void Update()
    {
        //if (isDashing) { return; }
    }

    private void FixedUpdate()
    {
        if (isDashing) { return; }
        Move();
        Flip();
    }

    //-------------------------------------------------------------
    // MOVEMENT
    //-------------------------------------------------------------
    private void Move()
    {
        if (moveInput != Vector3.zero)
        {
            rb.velocity = moveInput * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, idleSlow);
        }
    }

    private IEnumerator Dash() // TODO: Make player unable to be damaged when dashing
    {
        canDash = false;
        isDashing = true;
        rb.velocity = moveInput * (dashDistance / dashDuration);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ---------------------------------
    // SPRITE & ANIMATIONS
    // ---------------------------------
    private void Flip()
    {
        if (isFacingRight && moveInput.x < 0 || !isFacingRight && moveInput.x > 0)
        {
            isFacingRight = !isFacingRight;
        }

        spriteRenderer.flipX = isFacingRight;
    }

    //-------------------------------------------------------------
    // INPUT CHECKS
    //-------------------------------------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }
}
