using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

    // Movement
    [SerializeField, Range(0, 40)] private float baseSpeed = 10f;

    [Header("DASH")]
    [SerializeField] private float dashMultiplier = 8f;
    [SerializeField] private float dashDuration = 0.06f;
    [SerializeField] private float dashCooldown = 0.4f;

    [Header("SLIDE")]
    [SerializeField] private float slideSpeedMultiplier = 3f;

    [Header("JUMP")]
    [SerializeField] private float gravity = 25f;
    [SerializeField] private float jumpForce = 15f;

    [Header("GRAVITY IN FREE FALL")]
    [SerializeField] private float groundedGravity = -5f;

    [Header("STOMP")]
    [SerializeField] private float stompForce = 40f;

    // States
    private bool canDash = true;
    private bool jumpInput, dashInput, slideInputHeld;
    private bool dashing = false, sliding = false, stomping = false;
    private float fallVelocity;
    private Vector3 axis, movePlayer, dashDirection, slideDirection;
    public ParticleSystem speedParticles;
    // Crouch config
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
        HandleInput();
        HandleMovement();
        HandleGravity();
        HandleSlideEnd();

        player.Move(movePlayer * Time.deltaTime);
    }

    private void HandleInput()
    {
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetButtonDown("Sprint");
        slideInputHeld = Input.GetButton("Crouch");

        // Activar stomp solo en el aire
        if (slideInputHeld && !player.isGrounded && !stomping)
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
            ProcessSlide();
        }
        else
        {
            ProcessNormalMovement(rawMovement);
        }
    }

    private void ProcessNormalMovement(Vector3 rawMovement)
    {
        movePlayer.x = rawMovement.x * baseSpeed;
        movePlayer.z = rawMovement.z * baseSpeed;

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

    //========== DASH ==========
    private void StartDash(Vector3 direction)
    {
        dashing = true;
        canDash = false;
        dashDirection = direction.normalized * baseSpeed * dashMultiplier;
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ProcessDash()
    {
        movePlayer = dashDirection;
    }

    private void EndDash()
    {
        dashing = false;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    //========== SLIDE ==========
    private void StartSlide(Vector3 direction)
    {
        sliding = true;
        slideDirection = direction.normalized * baseSpeed * slideSpeedMultiplier;
        player.height = crouchHeight;
        player.center = new Vector3(player.center.x, crouchCenterY, player.center.z);

        // Activar el sistema de partículas cuando se inicia el deslizamiento
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

        // Desactivar el sistema de partículas cuando termina el deslizamiento
        if (speedParticles != null)
        {
            speedParticles.Stop();
        }
    }

    //========== GRAVITY ==========
    private void HandleGravity()
    {
        if (player.isGrounded)
        {
            if (stomping)
            {
                stomping = false;
                // Aquí podrías añadir un efecto de impacto
            }

            if (!jumpInput)
            {
                fallVelocity = groundedGravity;
            }

            if (jumpInput && !sliding && !dashing)
            {
                fallVelocity = jumpForce;
            }
        }
        else
        {
            if (!stomping)
            {
                fallVelocity -= gravity * Time.deltaTime;
            }
            // Si está en stomp, mantiene caída rápida constante
        }

        movePlayer.y = fallVelocity;
    }

    //========== STOMP ==========
    private void StartStomp()
    {
        stomping = true;
        fallVelocity = -stompForce;
    }
}