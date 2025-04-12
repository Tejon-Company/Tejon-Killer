using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

    // Movement variables
    [SerializeField, Range(0, 40)] private float baseSpeed = 10f;
    [Header("DASH")]
    [SerializeField] private float dashMultiplier = 8f;
    [SerializeField] private float dashDuration = 0.06f;
    [SerializeField] private float dashCooldown = 0.4f;
    [Header("SLIDE")]
    [Header("JUMP")]
    [SerializeField] private float gravity = 25f;
    [SerializeField] private float jumpForce = 15f;

    // State variables
    private bool canDash = true;
    private bool jumpInput, dashInput, slideInputHeld;
    private bool dashing = false, sliding = false;
    private float fallVelocity;
    private Vector3 axis, movePlayer, dashDirection;
    private float dashTimer = 0f;

    // Crouch variables
    private float originalHeight, crouchHeight = 1f;
    private float originalCenterY, crouchCenterY = 0.5f;

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
        HandleSlide(); // Llamada para manejar el deslizamiento

        player.Move(movePlayer * Time.deltaTime);
    }

    private void HandleInput()
    {
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetButtonDown("Sprint");
        slideInputHeld = Input.GetButton("Crouch");
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
            // No se hace nada en el movimiento, ya que el slide modifica la altura solo
            //ProcessSlide();
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

        // Iniciar dash
        if (dashInput && canDash && axis.magnitude > 0.1f)
        {
            StartDash(rawMovement);
        }
        // Iniciar deslizamiento
        else if (player.isGrounded && slideInputHeld)
        {
            StartSlide();
        }
    }

    //========== DASH ============
    private void StartDash(Vector3 direction)
    {
        dashing = true;
        dashTimer = dashDuration;
        canDash = false;
        dashDirection = direction.normalized * baseSpeed * dashMultiplier;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ProcessDash()
    {
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
        {
            EndDash();
        }
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

    //========== SLIDE ============
    private void StartSlide()
    {
        // Ajustar altura y centro del CharacterController al presionar la tecla de crouch
        player.height = crouchHeight;
        player.center = new Vector3(player.center.x, crouchCenterY, player.center.z);
    }

    private void EndSlide()
    {
        // Restaurar altura y centro originales cuando se suelta la tecla de crouch
        player.height = originalHeight;
        player.center = new Vector3(player.center.x, originalCenterY, player.center.z);
    }

    private void HandleSlide()
    {
        if (slideInputHeld)
        {
            // Mantener el deslizamiento mientras se mantenga presionada la tecla de crouch
            if (!sliding)
            {
                sliding = true;
                StartSlide();
            }
        }
        else
        {
            // Cuando se suelta la tecla, restaurar la altura
            if (sliding)
            {
                sliding = false;
                EndSlide();
            }
        }
    }

    //========== GRAVEDAD =========
    private void HandleGravity()
    {
        if (player.isGrounded)
        {
            fallVelocity = -10f;
            if (jumpInput && !sliding && !dashing)
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