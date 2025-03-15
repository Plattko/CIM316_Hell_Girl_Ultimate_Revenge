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

    private bool isFacingRight = true;

    private Coroutine dashCoroutine;
    private bool canDash = true;
    public bool isDashing { get; private set; }
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerAnimationController animationController;

    [Header("Combat")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private SpellManager spellManager;
    private Weapon weapon;

    [Header("Interaction")]
    [SerializeField] private Interactor interactor;

    [Header("Audio")]
    [SerializeField] private AudioClip[] dashWooshSFX;
    [SerializeField] private AudioClip[] dashGruntSFX;
    private float dashGruntChance = 0.33f;
    private int maxDashWithoutGrunt = 2;
    private int dashWithoutGruntCounter = 0;

    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        //weapon.SetPlayerController(this);
    }

    private void OnEnable()
    {
        if (spellManager != null) { spellManager.onSpellCast += OnSpellCast; }
        else { Debug.LogWarning("No Spell Manager detected."); }
        
        if (weaponManager != null) { weaponManager.onAttackStateChanged += OnAttackStateChanged; }
        else { Debug.LogWarning("No Weapon Manager detected."); }
    }

    private void OnDisable()
    {
        if (spellManager != null) { spellManager.onSpellCast -= OnSpellCast; }
        else { Debug.LogWarning("No Spell Manager detected."); }

        if (weaponManager != null) { weaponManager.onAttackStateChanged -= OnAttackStateChanged; }
        else { Debug.LogWarning("No Weapon Manager detected."); }
    }

    private void Update()
    {
        // Set the move speed in the animation controller
        animationController.SetMoveSpeed(rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        // Do nothing if the player can't move or is dashing
        if (canMove && !isDashing)
        {
            // Update the player's movement
            Move();
            // Update the player's facing direction
            Flip();
        }
    }

    private void OnAttackStateChanged(bool isAttacking)
    {
        animationController.SetIsAttacking(isAttacking);
    }

    private void OnSpellCast(float castTime, bool lockoutDuringCast)
    {
        // Disable movement for the duration of the cast if the spell has a lockout during its cast
        if (lockoutDuringCast)
        {
            StartCoroutine(DisableMovementTemp(castTime));
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

    private IEnumerator Dash() // TODO: Make player unable to be damaged when dashing
    {
        // Disable the ability to dash
        canDash = false;
        // Set dashing to true
        isDashing = true;
        // Tell the animation controller the player is dashing
        animationController.SetIsDashing(true);
        // Play the dash woosh SFX
        SFXManager.Instance.PlayRandomAudioClip(dashWooshSFX, transform, 1.1f, 1f, true);
        // Have a chance of playing the dash grunt SFX and guarantee it plays after a certain number of dashes without it playing
        if (Random.value < dashGruntChance || dashWithoutGruntCounter >= maxDashWithoutGrunt)
        {
            SFXManager.Instance.PlayRandomAudioClip(dashGruntSFX, transform, 0.5f, 1f, true, 0.05f);
            dashWithoutGruntCounter = 0;
        }
        else
        {
            dashWithoutGruntCounter++;
        }
        // Set the player's velocity to speed required to travel the dash's distance over its duration in the direction of the move input
        rb.velocity = moveInput * (dashDistance / dashDuration);
        // Wait for the dash duration and set dashing to false
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        // Tell the animation controller the player is no longer dashing
        animationController.SetIsDashing(false);
        // Wait for the dash cooldown duration and re-enable the ability to dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void InterruptDash()
    {
        if (dashCoroutine != null)
        {
            // Stop the dash coroutine
            StopCoroutine(dashCoroutine);
            // Reset dash variables
            isDashing = false;
            canDash = true;
            // Tell the animation controller the player is no longer dashing
            animationController.SetIsDashing(false);
        }
    }

    public void DisableMovement()
    {
        // Disable movement
        canMove = false;
        // Interrupt the dash
        InterruptDash();
        // Set the player's velocity to 0
        rb.velocity = Vector3.zero;
    }

    public void EnableMovement()
    {
        // Enable movement
        canMove = true;
    }

    private IEnumerator DisableMovementTemp(float duration)
    {
        // Disable movement
        canMove = false;
        // Interrupt the dash
        InterruptDash();
        // Set the player's velocity to 0
        rb.velocity = Vector3.zero;
        // Wait for the lockout duration
        yield return new WaitForSeconds(duration);
        // Re-enable movement
        canMove = true;
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
        spriteRenderer.flipX = !isFacingRight;
    }

    //-------------------------------------------------------------
    // INPUT CHECKS
    //-------------------------------------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        // Set the move input on the x and z axis
        moveInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        // Set the move pressed bool in the animation controller
        bool isMovePressed = moveInput != Vector3.zero;
        animationController.SetIsMovePressed(isMovePressed);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // If the input is pressed and the player can dash, dash
        if (context.performed && canMove && canDash)
        {
            dashCoroutine = StartCoroutine(Dash());
        }
    }

    public void OnUseWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weaponManager.StartAttacking();
        }

        if (context.canceled)
        {
            weaponManager.StopAttacking();
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

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.TogglePause();
        }
    }
}
