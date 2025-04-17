using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

    [SerializeField, Range(0, 40)] private float baseSpeed = 10f;

    [Header("DASH")]
    [SerializeField] private float dashMultiplier = 10f;
    [SerializeField] private float dashDuration = 0.08f;
    [SerializeField] private float dashCooldown = 0.4f;

    [Header("SLIDE")]
    [SerializeField] private float slideSpeedMultiplier = 2.5f;
    [SerializeField] private float slideGravity = 14f;
    [SerializeField] private float slideJumpForce = 20f;
    [SerializeField] private float slideJumpInertiaMultiplier = 2f;
    private bool slideJumpInertiaActive = false;
    private bool canSlideJump;
    private bool skipGravityNextFrame = false;

    [Header("JUMP")]
    [SerializeField] private float gravity = 25f;
    [SerializeField] private float jumpForce = 15f;


    [Header("MULTI JUMP")]
    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("GRAVITY IN FREE FALL")]
    [SerializeField] private float groundedGravity = -5f;

    [Header("STOMP")]
    [SerializeField] private float stompForce = 40f;
    [SerializeField] private float stompTimeLimit = 0.3f;
    [SerializeField] private float stompJumpForceMultiplier = 1.5f;
    private float stompTimeCounter = 0f;

    [Header("JUMP BUFFER")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter = 0f;

    [SerializeField] private Sway weaponSway;
    [SerializeField] private ParticleSystem speedParticles;
    [HideInInspector] public bool dashing = false, sliding = false, stomping = false;
    private bool canDash = true;
    // Ahora usamos jumpInput de forma explícita:
    private bool jumpInput, dashInput, slideInputHeld;
    private float fallVelocity;
    private Vector3 axis, movePlayer, dashDirection, slideDirection;

    private float originalHeight, crouchHeight = 1f;
    private float originalCenterY, crouchCenterY = 0.5f;

    private void Awake()
    {
        speedParticles.Stop();
        player = GetComponent<CharacterController>();
        originalHeight = player.height;
        originalCenterY = player.center.y;
    }

    private void Update()
    {
        if (stompTimeCounter > 0)
        {
            stompTimeCounter -= Time.deltaTime;
        }
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        HandleInput();
        HandleMovement();
        if (!skipGravityNextFrame)
        {
            HandleGravity();
        }
        skipGravityNextFrame = false;
        HandleSlideEnd();

        player.Move(movePlayer * Time.deltaTime);
    }

    private void HandleInput()
    {
        // Actualizamos jumpInput de forma explícita
        jumpInput = Input.GetButtonDown("Jump") && jumpsRemaining > 0;
        if (jumpInput)
        {
            jumpBufferCounter = jumpBufferTime;
            weaponSway.TriggerJumpEffect();
        }

        dashInput = Input.GetButtonDown("Sprint");
        bool stompInput = Input.GetButtonDown("Crouch");

        slideInputHeld = Input.GetButton("Crouch");

        if (stompInput && !player.isGrounded && !stomping && !sliding)
        {
            StartStomp();
        }
    }

private void HandleMovement()
{
    axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    axis = axis.magnitude > 1 ? axis.normalized : axis;
    Vector3 rawMovement = transform.TransformDirection(axis);

    if (dashing)
    {
        ProcessDash();
    }
    else if (sliding)
    {
        // 1) Detectar el botón justo en el momento de pulsarse
        if (canSlideJump && Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            EndSlide();
            slideDirection.y = 0f;

            // impulso potenciado
            Vector3 jumpImpulse = slideDirection * slideJumpInertiaMultiplier;
            movePlayer = jumpImpulse;
            fallVelocity = slideJumpForce;
            movePlayer.y = fallVelocity;

            slideJumpInertiaActive = true;

            // ya no podemos volver a slide‑jumpear hasta el próximo slide
            canSlideJump = false;
            skipGravityNextFrame = true;
            return;
        }

        // 2) Si no salta, seguimos deslizando
        ProcessSlide();
    }
    else
    {
        ProcessNormalMovement(rawMovement);
    }

    // actualizar componente vertical
    movePlayer.y = fallVelocity;
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

        if (player.isGrounded && slideInputHeld && axis.magnitude > 0.1f && !sliding)
        {
            StartSlide(rawMovement);
        }
    }



    private void StartDash(Vector3 direction)
    {
        dashing = true;
        canDash = false;
        dashDirection = direction.normalized * baseSpeed * dashMultiplier;
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
        if (speedParticles != null)
        {
            speedParticles.Play();
        }
    }

    private void ProcessDash()
    {
        movePlayer = dashDirection;
    }

    private void EndDash()
    {
        dashing = false;
        if (speedParticles != null && !sliding)
        {
            speedParticles.Stop();
        }
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void StartSlide(Vector3 direction)
    {
        sliding = true;
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
        if (!slideInputHeld && sliding)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        sliding = false;
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
            slideJumpInertiaActive = false;
            if (stomping)
            {
                stompTimeCounter = stompTimeLimit;
                stomping = false;
            }
            else
            {
                jumpsRemaining = maxJumps;
            }

            if (jumpBufferCounter > 0 && !stomping)
            {
                if (stompTimeCounter > 0)
                {
                    sliding=false;
                    speedParticles.Stop();
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
            else if (fallVelocity <= 0f)
            {
                fallVelocity = groundedGravity;
            }
        }
        else
        {
            if (jumpBufferCounter > 0 && jumpsRemaining > 0 && !stomping)
            {
                fallVelocity = jumpForce;
                jumpsRemaining--;
                jumpBufferCounter = 0f;
            }

            if (player.collisionFlags == CollisionFlags.Above && fallVelocity > 0)
            {
                fallVelocity = -1f;
            }

            if (!stomping)
            {
                fallVelocity -= (sliding ? slideGravity : gravity) * Time.deltaTime;
            }

            if (stompTimeCounter > 0)
            {
                stompTimeCounter -= Time.deltaTime;
            }
        }

        movePlayer.y = fallVelocity;
    }

    private void StartStomp()
    {
        weaponSway.TriggerStompEffect();
        stomping = true;
        fallVelocity = -stompForce;
    }

    public Vector3 GetSlideDirection()
    {
        return slideDirection;
    }
}
