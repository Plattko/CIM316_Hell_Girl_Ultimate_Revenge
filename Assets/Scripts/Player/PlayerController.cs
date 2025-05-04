using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody rb;

    private Vector3 moveInput;
    private Vector3 lastMoveInput = Vector3.right;
    private bool canMove = true;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float idleSlow = 0.9f;

    public bool isFacingRight { get; private set; } = true;

    private Coroutine dashCoroutine;
    private bool canDash = true;
    public bool isDashing { get; private set; }
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private GameObject afterImageGenerator;
    [SerializeField] private SpriteRenderer itemDisplaySpriteRenderer;
    public enum FlipType { OnMove, OnAttack, }

    [Header("Combat")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private WeaponGenerator weaponGenerator;
    [SerializeField] private SpellManager spellManager;
    private Weapon weapon;
    private bool isAttacking;

    [field: SerializeField] public Transform WeaponAimPivot { get; private set; }

    private bool canAttack = true;

    [Header("Interaction")]
    [SerializeField] private Interactor interactor;
    private float itemPickupDuration = 1.25f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] dashWooshSFX;
    [SerializeField] private AudioClip[] dashGruntSFX;
    private float dashGruntChance = 0.33f;
    private int maxDashWithoutGrunt = 2;
    private int dashWithoutGruntCounter = 0;

    // TEMPORARY
    [Header("Temporary")]
    [SerializeField] private PlayerAim playerAim;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.SetPlayerController(this);
    }

    private void OnEnable()
    {
        playerHealth.onDied += OnDied;
        weaponManager.onAttackStateChanged += OnAttackStateChanged;
        weaponGenerator.onWeaponPickedUp += RegularItemPickup;
        spellManager.onSpellCast += OnSpellCast;
        spellManager.onSpellPickedUp += RegularItemPickup;
    }

    private void OnDisable()
    {
        playerHealth.onDied -= OnDied;
        weaponManager.onAttackStateChanged -= OnAttackStateChanged;
        weaponGenerator.onWeaponPickedUp -= RegularItemPickup;
        spellManager.onSpellCast -= OnSpellCast;
        spellManager.onSpellPickedUp -= RegularItemPickup;
    }

    private void Update()
    {
        // Set the move speed in the animation controller
        animationController.SetMoveSpeed(rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (canMove && !isDashing && !isAttacking)
        {
            // Update the player's movement
            Move();
            // Update the player's facing direction
            Flip(FlipType.OnMove);
        }
    }

    private void OnAttackStateChanged(bool isAttacking)
    {
        animationController.SetIsAttacking(isAttacking);
        this.isAttacking = isAttacking;
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

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        // Tell the animation controller the player is dashing
        animationController.SetIsDashing(true);
        // Start spawning after images
        afterImageGenerator.SetActive(true);
        // Play the dash woosh SFX
        SFXManager.Instance.PlayRandomAudioClip(dashWooshSFX, transform, 1.1f, 1f, true);
        // Have a chance of playing the dash grunt SFX and guarantee it plays after a certain number of dashes without it playing
        if (Random.value < dashGruntChance || dashWithoutGruntCounter >= maxDashWithoutGrunt)
        {
            SFXManager.Instance.PlayRandomAudioClip(dashGruntSFX, transform, 0.4f, 1f, true, 0.05f);
            dashWithoutGruntCounter = 0;
        }
        else
        {
            dashWithoutGruntCounter++;
        }
        // Set the player's velocity to speed required to travel the dash's distance over its duration in the direction of the move input
        rb.velocity = lastMoveInput * (dashDistance / dashDuration);
        // Wait for the dash duration and set dashing to false
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        // Tell the animation controller the player is no longer dashing
        animationController.SetIsDashing(false);
        // Stop spawning after images
        afterImageGenerator.SetActive(false);
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
            // Stop spawning after images
            afterImageGenerator.SetActive(false);
        }
    }

    public void ToggleMovement(bool enabled)
    {
        canMove = enabled;
        
        if (!enabled)
        {
            // Interrupt the dash
            InterruptDash();
            // Set the player's velocity to 0
            SetVelocityZero();
        }
    }

    private IEnumerator DisableMovementTemp(float duration)
    {
        // Disable movement
        canMove = false;
        // Interrupt the dash
        InterruptDash();
        // Set the player's velocity to 0
        SetVelocityZero();
        // Wait for the lockout duration
        yield return new WaitForSeconds(duration);
        // Re-enable movement
        canMove = true;
    }

    public void SetVelocity(float velocity, Vector3 direction)
    {
        rb.velocity = direction * velocity;
    }

    public void SetVelocityZero()
    {
        rb.velocity = Vector3.zero;
    }

    //-------------------------------------------------------------
    // ATTACKS
    //-------------------------------------------------------------
    public void ToggleAttacks(bool enabled)
    {
        canAttack = enabled;
        WeaponAimPivot.gameObject.SetActive(enabled);
    }

    //-------------------------------------------------------------
    // PICKING UP ITEMS
    //-------------------------------------------------------------
    private void RegularItemPickup(Item item)
    {
        StartCoroutine(StartItemPickup(item, true));
    }

    public IEnumerator StartItemPickup(Item item, bool endsAutomatically)
    {
        SFXManager.Instance.PlayAudioClip(item.equipSFX, transform, 1f);
        itemDisplaySpriteRenderer.sprite = item.icon;
        itemDisplaySpriteRenderer.enabled = true;
        UIManager.Instance.SetItemBanner(item.name, item.description);
        UIManager.Instance.FadeInItemBanner();
        ToggleMovement(false);
        ToggleAttacks(false);
        animationController.SetIsItemPickedUp(true);

        if (endsAutomatically)
        {
            yield return new WaitForSeconds(itemPickupDuration);
            EndItemPickup();
        }
        yield return null;
    }

    public void EndItemPickup()
    {
        itemDisplaySpriteRenderer.enabled = false;
        UIManager.Instance.FadeOutItemBanner();
        animationController.SetIsItemPickedUp(false);
        if (!GameManager.Instance.IsInOnboarding)
        {
            ToggleMovement(true);
            ToggleAttacks(true);
        }
    }

    //-------------------------------------------------------------
    // DYING
    //-------------------------------------------------------------
    public void OnDied()
    {
        // Shift sprite object so Helena is still centred
        Vector3 pos = spriteRenderer.transform.position;
        float offset = spriteRenderer.flipX ? -0.45f : 0.45f;
        spriteRenderer.transform.position = new Vector3(pos.x + offset, pos.y, pos.z);
        // Disable movement and attacks
        ToggleMovement(false);
        ToggleAttacks(false);
        // Tell the animation controller the player is dead
        animationController.SetIsDead(true);
    }

    //-------------------------------------------------------------
    // SPRITE & ANIMATIONS
    //-------------------------------------------------------------
    private void Flip(FlipType flipType)
    {
        switch (flipType)
        {
            case FlipType.OnMove:
                // If the player changes direction, update their facing direction
                if (isFacingRight && moveInput.x < 0 || !isFacingRight && moveInput.x > 0)
                {
                    isFacingRight = !isFacingRight;
                }
                // Flip the player's sprite in the direction they are facing
                spriteRenderer.flipX = !isFacingRight;
                break;

            case FlipType.OnAttack:
                // Get the mouse's world position
                var (success, position) = playerAim.GetMouseWorldPosition();
                // Do nothing if getting the mouse's world position was unsuccessful
                if (!success) return;
                // Get the player's aim direction
                bool isAimingRight = position.x > transform.position.x;
                // Set the player's facing direction to their aim direction
                isFacingRight = isAimingRight;

                // Flip the player's sprite, weapon sprite and temp hitbox in the direction they are facing
                spriteRenderer.flipX = !isFacingRight;
                weaponSpriteRenderer.flipX = !isFacingRight;
                break;

            default:
                break;
        }
    }

    //-------------------------------------------------------------
    // INPUT CHECKS
    //-------------------------------------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        // Set the move input on the x and z axis
        Vector3 input = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        moveInput = input;
        // Set the last move input to the last non-zero input
        if (input != Vector3.zero)
        {
            lastMoveInput = input;
        }
        // Set the move pressed bool in the animation controller
        bool isMovePressed = moveInput != Vector3.zero;
        animationController.SetIsMovePressed(isMovePressed);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePaused) return;
        
        if (context.performed && canMove && canDash && !isAttacking)
        {
            dashCoroutine = StartCoroutine(Dash());
        }
    }

    public void OnUseWeapon(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePaused) return;
        
        if (!canAttack) return;
        
        if (context.performed && !isDashing)
        {
            Flip(FlipType.OnAttack);
            weaponManager.StartAttacking();
        }

        if (context.canceled)
        {
            weaponManager.StopAttacking();
        }
    }

    public void OnUseSpell(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePaused) return;
        
        if (!canAttack) return;

        if (context.performed && !isDashing && !isAttacking)
        {
            spellManager.CastSpell();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePaused) return;

        if (context.performed && canMove && !isAttacking)
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
