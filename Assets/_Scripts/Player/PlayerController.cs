using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;
    private bool _dashing = false;
    private bool _sliding = false;
    private bool _stomping = false;
    public bool IsDashing
    {
        get { return _dashing; }
    }
    public bool IsSliding
    {
        get { return _sliding; }
    }
    public bool IsStomping
    {
        get { return _stomping; }
    }

    [SerializeField, Range(0, 40)]
    private float baseSpeed = 10f;

    [Header("DASH")]
    [SerializeField]
    private float dashMultiplier = 10f;

    [SerializeField]
    private float dashDuration = 0.08f;

    [SerializeField]
    private float dashCooldown = 0.4f;

    [SerializeField]
    private ParticleSystem dashParticles;

    [SerializeField]
    private ParticleSystem speedParticles;

    [Header("SLIDE")]
    [SerializeField]
    private float slideSpeedMultiplier = 2.5f;

    [SerializeField]
    private float slideGravity = 14f;

    [SerializeField]
    private float slideJumpForce = 20f;

    [SerializeField]
    private float slideJumpInertiaMultiplier = 2f;
    private bool slideJumpInertiaActive = false;
    private bool canSlideJump;
    private bool skipGravityNextFrame = false;

    [Header("JUMP")]
    [SerializeField]
    private float gravity = 25f;

    [SerializeField]
    private float jumpForce = 15f;

    [Header("MULTI JUMP")]
    [SerializeField]
    private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("GRAVITY IN FREE FALL")]
    [SerializeField]
    private float groundedGravity = -5f;

    [Header("STOMP")]
    [SerializeField]
    private float stompForce = 40f;

    [SerializeField]
    private float stompTimeLimit = 0.3f;

    [SerializeField]
    private float stompJumpForceMultiplier = 1.5f;
    private float stompTimeCounter = 0f;

    [SerializeField]
    private ParticleSystem stompParticles;

    [Header("JUMP BUFFER")]
    [SerializeField]
    private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter = 0f;

    private bool canDash = true;
    private bool jumpInput,
        dashInput,
        slideInputHeld;
    private float fallVelocity;
    private Vector3 axis,
        movePlayer,
        dashDirection,
        slideDirection;
    private float originalHeight,
        crouchHeight = 1f;
    private float originalCenterY,
        crouchCenterY = 0.5f;
    private Sway weaponSway;
    private bool wasGrounded;
    private bool cond1;
    private bool cond2;

    private void Awake()
    {
        speedParticles.Stop();
        dashParticles.Stop();
        stompParticles.Stop();
        player = GetComponent<CharacterController>();
        originalHeight = player.height;
        originalCenterY = player.center.y;
        wasGrounded = player.isGrounded;
    }

    private void Update()
    {
        UpdateTimers();

        bool groundedNow = player.isGrounded;
        bool justLanded = !wasGrounded && groundedNow;

        HandleInput();
        HandleMovement();

        if (justLanded)
            OnLand();

        if (!skipGravityNextFrame)
            HandleGravity();

        skipGravityNextFrame = false;

        HandleSlideEnd();

        player.Move(movePlayer * Time.deltaTime);
        wasGrounded = groundedNow;
    }

    private void UpdateTimers()
    {
        if (stompTimeCounter > 0f)
            stompTimeCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;
    }

    private void OnLand()
    {
        jumpsRemaining = maxJumps;
        slideJumpInertiaActive = false;
        stompTimeCounter = 0f;

        if (stompParticles != null)
        {
            stompParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void HandleInput()
    {
        jumpInput = Input.GetButtonDown("Jump") && jumpsRemaining > 0;
        if (jumpInput)
        {
            jumpBufferCounter = jumpBufferTime;
            weaponSway?.TriggerJumpEffect();
        }

        dashInput = Input.GetButtonDown("Sprint");
        slideInputHeld = Input.GetButton("Crouch");

        bool stompInput = Input.GetButtonDown("Crouch");
        if (ShouldStomp(stompInput))
            StartStomp();
    }

    private bool ShouldStomp(bool stompInput)
    {
        return stompInput && !player.isGrounded && !_stomping && !_sliding;
    }

    private void HandleMovement()
    {
        UpdateMovementInput();

        if (_dashing)
        {
            ProcessDash();
            movePlayer.y = fallVelocity;
            return;
        }

        if (_sliding)
        {
            if (CanPerformSlideJump())
            {
                PerformSlideJump();
                return;
            }

            ProcessSlide();
            movePlayer.y = fallVelocity;
            return;
        }

        ProcessNormalMovement(transform.TransformDirection(axis));
        movePlayer.y = fallVelocity;
    }

    private void UpdateMovementInput()
    {
        axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (axis.magnitude > 1f)
            axis.Normalize();
    }

    private bool CanPerformSlideJump()
    {
        return canSlideJump && jumpBufferCounter > 0f && jumpsRemaining > 0;
    }

    private void PerformSlideJump()
    {
        EndSlide();
        slideDirection.y = 0f;
        jumpsRemaining--;

        Vector3 jumpImpulse = slideDirection * slideJumpInertiaMultiplier;
        movePlayer = jumpImpulse;
        fallVelocity = slideJumpForce;
        movePlayer.y = fallVelocity;

        slideJumpInertiaActive = true;
        canSlideJump = false;
        skipGravityNextFrame = true;

        jumpBufferCounter = 0f;
    }

    private void ProcessNormalMovement(Vector3 rawMovement)
    {
        float currentSpeed = baseSpeed;

        if (slideJumpInertiaActive && !player.isGrounded)
        {
            currentSpeed *= slideJumpInertiaMultiplier;
        }

        movePlayer.x = rawMovement.x * currentSpeed;
        movePlayer.z = rawMovement.z * currentSpeed;

        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);

        if (dashInput && canDash && axis.magnitude > 0.1f)
        {
            StartDash(rawMovement);
        }

        if (player.isGrounded && slideInputHeld && axis.magnitude > 0.1f && !_sliding)
        {
            StartSlide(rawMovement);
        }
    }

    public void SetWeaponSway(Sway newSway)
    {
        weaponSway = newSway;
    }

    private void StartDash(Vector3 direction)
    {
        _dashing = true;
        canDash = false;
        dashDirection = direction.normalized * baseSpeed * dashMultiplier;
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
        if (dashParticles != null)
        {
            dashParticles.Play();
        }
    }

    private void ProcessDash()
    {
        movePlayer = dashDirection;
    }

    private void EndDash()
    {
        _dashing = false;
        if (dashParticles != null)
        {
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void StartSlide(Vector3 direction)
    {
        _sliding = true;
        canSlideJump = true;
        slideDirection = direction.normalized * baseSpeed * slideSpeedMultiplier;
        player.height = crouchHeight;
        player.center = new Vector3(player.center.x, crouchCenterY, player.center.z);

        if (speedParticles != null)
        {
            speedParticles.Play();
        }
    }

    private void ProcessSlide()
    {
        movePlayer.x = slideDirection.x;
        movePlayer.z = slideDirection.z;
    }

    private void HandleSlideEnd()
    {
        if (!slideInputHeld && _sliding)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        _sliding = false;
        player.height = originalHeight;
        player.center = new Vector3(player.center.x, originalCenterY, player.center.z);
        if (speedParticles != null)
        {
            speedParticles.Stop();
        }
    }

    private void HandleGravity()
    {
        if (player.isGrounded)
        {
            HandleGroundedGravity();
        }
        else
        {
            HandleAirborneGravity();
        }

        movePlayer.y = fallVelocity;
    }

    private void HandleGroundedGravity()
    {
        slideJumpInertiaActive = false;
        jumpsRemaining = maxJumps;

        if (_stomping)
        {
            CancelStomp();
            return;
        }

        if (jumpBufferCounter <= 0)
        {
            if (fallVelocity <= 0f)
                fallVelocity = groundedGravity;
            return;
        }

        if (stompTimeCounter > 0f)
        {
            EndSlide();
            fallVelocity = jumpForce * stompJumpForceMultiplier;
            jumpsRemaining = maxJumps - 1;
            stompTimeCounter = 0f;
        }
        else
        {
            fallVelocity = jumpForce;
            jumpsRemaining--;
        }

        jumpBufferCounter = 0f;
    }

    private void HandleAirborneGravity()
    {
        bool cond1 = jumpBufferCounter > 0 && jumpsRemaining > 0 && !_stomping;
        bool cond2 = player.collisionFlags == CollisionFlags.Above && fallVelocity > 0;
        if (cond1)
        {
            fallVelocity = jumpForce;
            jumpsRemaining--;
            jumpBufferCounter = 0f;
        }

        if (cond2)
        {
            fallVelocity = -1f;
        }

        if (!_stomping)
        {
            fallVelocity -= (_sliding ? slideGravity : gravity) * Time.deltaTime;
        }

        if (stompTimeCounter > 0)
        {
            stompTimeCounter -= Time.deltaTime;
        }
    }

    private void CancelStomp()
    {
        stompTimeCounter = stompTimeLimit;
        _stomping = false;
    }

    private void StartStomp()
    {
        weaponSway?.TriggerStompEffect();
        _stomping = true;
        fallVelocity = -stompForce;
        if (stompParticles != null)
        {
            stompParticles.Play();
        }
    }

    public Vector3 GetSlideDirection()
    {
        return slideDirection;
    }
}
