using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

    [SerializeField, Range(0, 40)]
    private float baseSpeed = 10f;

    [SerializeField]
    private float dashMultiplier = 8f;
    [SerializeField]
    private float dashDuration = 0.06f;
    [SerializeField]
    private float dashCooldown = 1f;
    private bool canDash = true;

    [SerializeField]
    private float slideMultiplier = 2.5f;
    [SerializeField]
    private float slideSpeedDecay = 0.95f;

    [SerializeField]
    private float gravity = 25f, jumpForce = 15f;

    private float fallVelocity;
    private Vector3 axis, movePlayer;
    private bool jumping, dashInput;
    private bool slideInputHeld;
    private bool dashing = false;
    private bool sliding = false;

    private float originalHeight;
    private float crouchHeight = 1f;
    private float originalCenterY;
    private float crouchCenterY = 0.5f;

    private float dashTimer = 0f;
    private Vector3 dashDirection;
    private Vector3 slideDirection;
    private float currentSlideSpeed;

    private void Awake()
    {
        player = GetComponent<CharacterController>();
        originalHeight = player.height;
        originalCenterY = player.center.y;
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleGravity();
        
        player.Move(movePlayer * Time.deltaTime);
    }

    private void HandleInput()
    {
        jumping = Input.GetButtonDown("Jump");
        dashInput = Input.GetButtonDown("Sprint");
        slideInputHeld = Input.GetButton("Crouch");
    }

    private void HandleMovement()
    {
        axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        axis = axis.magnitude > 1 ? axis.normalized : axis;
        
        // Movimiento base sin aplicar todavía la velocidad
        Vector3 rawMovement = transform.TransformDirection(axis);

        if (dashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                EndDash();
            }
            movePlayer = dashDirection;
        }
        else if (sliding)
        {
            // Aplicar decaimiento en la velocidad del deslizamiento
            currentSlideSpeed *= slideSpeedDecay;
            movePlayer = slideDirection * currentSlideSpeed;

            // Finalizar deslizamiento si se suelta la tecla o está en el aire
            if (!slideInputHeld || !player.isGrounded)
            {
                EndSlide();
            }
        }
        else
        {
            // Movimiento normal
            movePlayer.x = rawMovement.x * baseSpeed;
            movePlayer.z = rawMovement.z * baseSpeed;

            // Rotación solo cuando no se está deslizando
            transform.Rotate(0, Input.GetAxis("Mouse X"), 0);

            // Iniciar dash
            if (dashInput && canDash && axis.magnitude > 0.1f)
            {
                StartDash(rawMovement);
            }
            // Iniciar deslizamiento
            else if (player.isGrounded && slideInputHeld)
            {
                StartSlide(rawMovement);
            }
        }
    }

    private void StartDash(Vector3 direction)
    {
        dashing = true;
        dashTimer = dashDuration;
        canDash = false;
        dashDirection = direction.normalized * baseSpeed * dashMultiplier;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void EndDash()
    {
        dashing = false;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void StartSlide(Vector3 movementDirection)
    {
        sliding = true;

        // Ajustar altura y centro del CharacterController
        player.height = crouchHeight;
        player.center = new Vector3(player.center.x, crouchCenterY, player.center.z);

        // Congelar la dirección de deslizamiento (en plano XZ)
        slideDirection = movementDirection.magnitude > 0.1f ? 
            new Vector3(movementDirection.x, 0, movementDirection.z).normalized : 
            new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        currentSlideSpeed = baseSpeed * slideMultiplier;
    }

    private void EndSlide()
    {
        sliding = false;

        // Restaurar altura y centro originales
        player.height = originalHeight;
        player.center = new Vector3(player.center.x, originalCenterY, player.center.z);
    }

    private void HandleGravity()
    {
        if (player.isGrounded)
        {
            fallVelocity = -10f;
            if (jumping && !sliding && !dashing)
            {
                fallVelocity = jumpForce;
            }
        }
        else
        {
            fallVelocity -= gravity * Time.deltaTime;
        }

        movePlayer.y = fallVelocity;
    }
}