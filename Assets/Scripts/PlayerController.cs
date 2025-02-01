using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;

    private Vector3 moveInput;
    private bool canMove = true;
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
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Spellcasting")]
    [SerializeField] private SpellManager spellManager;

    [Header("Interacting")]
    [SerializeField] private Interactor interactor;

    private void OnEnable()
    {
        if (spellManager != null)
        {
            spellManager.onSpellCast += OnSpellCast;
        }
        else
        {
            Debug.LogWarning("No Spell Manager detected.");
        }
    }

    private void OnDisable()
    {
        if (spellManager != null)
        {
            spellManager.onSpellCast -= OnSpellCast;
        }
        else
        {
            Debug.LogWarning("No Spell Manager detected.");
        }
    }

    private void FixedUpdate()
    {
        // Do nothing if the player can't move or is dashing
        if (!canMove || isDashing) { return; }
        // Update the player's movement
        Move();
        // Update the player's facing direction
        Flip();
    }

    private void OnSpellCast(float castTime, bool lockoutDuringCast)
    {
        // Disable movement for the duration of the cast if the spell has a lockout during its cast
        if (lockoutDuringCast)
        {
            StartCoroutine(MovementLockout(castTime));
        }
    }

    //-------------------------------------------------------------
    // MOVEMENT
    //-------------------------------------------------------------
    private void Move()
    {
        // If there is move input, set the player's velocity to the move speed in the direction of the input
        if (moveInput != Vector3.zero)
        {
            rb.velocity = moveInput * moveSpeed;
        }
        // If there is no move input, lerp the player's velocity to 0 using the idle slow speed
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, idleSlow);
        }
    }

    private IEnumerator MovementLockout(float duration)
    {
        // Disable movement
        canMove = false;
        // Set the player's velocity to 0
        rb.velocity = Vector3.zero;
        // Wait for the lockout duration
        yield return new WaitForSeconds(duration);
        // Re-enable movement
        canMove = true;
    }

    private IEnumerator Dash() // TODO: Make player unable to be damaged when dashing
    {
        // Disable the ability to dash
        canDash = false;
        // Set dashing to true
        isDashing = true;
        // Set the player's velocity to speed required to travel the dash's distance over its duration in the direction of the move input
        rb.velocity = moveInput * (dashDistance / dashDuration);
        // Wait for the dash duration and set dashing to false
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        // Wait for the dash cooldown duration and re-enable the ability to dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //-------------------------------------------------------------
    // SPRITE & ANIMATIONS
    //-------------------------------------------------------------
    private void Flip()
    {
        // If the player changes direction, update their facing direction
        if (isFacingRight && moveInput.x < 0 || !isFacingRight && moveInput.x > 0)
        {
            isFacingRight = !isFacingRight;
        }
        // Flip the player's sprite in the direction they are facing
        spriteRenderer.flipX = isFacingRight;
    }

    //-------------------------------------------------------------
    // INPUT CHECKS
    //-------------------------------------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        // Set the move input on the x and z axis
        moveInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // If the input is pressed and the player can dash, dash
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void OnUseSpell(InputAction.CallbackContext context)
    {
        // If the input is pressed, cast a spell through the Spell Manager script
        if (context.performed)
        {
            spellManager.CastSpell();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactor.Interact();
        }
    }
}
