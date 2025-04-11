using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

    [SerializeField, Range(0, 40)]
    private float baseSpeed = 10f;

    [SerializeField]
    private float sprintMultiplier = 2f;

    [SerializeField]
    private float gravity = 5f, jumpForce = 3f;

    private float fallVelocity;
    private Vector3 axis, movePlayer;
    private bool jumping, sprinting;

    private void Awake()
    {
        player = GetComponent<CharacterController>();
    }

    private void Update()
    {
        jumping = Input.GetButtonDown("Jump");
        sprinting = Input.GetButton("Sprint"); // Usa tu input personalizado

        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);

        axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        axis = axis.magnitude > 1 ? transform.TransformDirection(axis.normalized) : transform.TransformDirection(axis);

        movePlayer.x = axis.x;
        movePlayer.z = axis.z;

        SetGravity();

        float currentSpeed = sprinting ? baseSpeed * sprintMultiplier : baseSpeed;

        player.Move(movePlayer * currentSpeed * Time.deltaTime);
    }

    private void SetGravity()
    {
        if (player.isGrounded)
        {
            fallVelocity = -gravity * Time.deltaTime;
            if (jumping)
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
