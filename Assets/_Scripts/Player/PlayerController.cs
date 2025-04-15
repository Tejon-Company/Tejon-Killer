using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

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

    [Header("WEAPON")]
    private Vector3 weaponOriginalLocalPos;
    private Vector3 weaponSlideOffset = new Vector3(0, 0, -0.3f);

    private bool canDash = true;
    private bool jumpInput, dashInput, slideInputHeld;
    public bool dashing = false, sliding = false, stomping = false;
    private float fallVelocity;
    private Vector3 axis, movePlayer, dashDirection, slideDirection;
    public ParticleSystem speedParticles;
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

            if (jumpInput && player.isGrounded)
            {
                EndSlide();
                fallVelocity = jumpForce;

                stomping = false;
            }
            else
            {
                ProcessSlide();
            }
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
            if (stomping)
            {
                stomping = false;
            }

            if (!jumpInput)
            {
                fallVelocity = groundedGravity;
            }
            else if (jumpInput)
            {
                fallVelocity = jumpForce;
            }
        }
        else
        {
            if (player.collisionFlags == CollisionFlags.Above && fallVelocity > 0)
            {
                fallVelocity = -1f; // fuerza el comienzo de la caída al tocar el techo
            }

            if (!stomping)
            {
                fallVelocity -= gravity * Time.deltaTime;
            }
        }

        movePlayer.y = fallVelocity;
    }

    private void StartStomp()
    {
        Debug.Log("STOMP");
        stomping = true;
        fallVelocity = -stompForce;
    }
}