using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController player;

    [SerializeField, Range(0, 20)]
    private float moveSpeed;

    [SerializeField]
    private float gravity, jumpForce;

    private float fallVelocity;
    private Vector3 axis, movePlayer;
    private bool jumpInput;
    private void Awake()
    {
        player = GetComponent<CharacterController>();
    }
    private void Update()
    {
        jumpInput = Input.GetButtonDown("Jump");
        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
        axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (axis.magnitude > 1) axis = transform.TransformDirection(axis).normalized;
        else axis = transform.TransformDirection(axis);

        movePlayer.x = axis.x;
        movePlayer.z = axis.z;
        SetGravity();

        player.Move(movePlayer * moveSpeed* Time.deltaTime);
    }

    private void SetGravity()
    {
        Debug.Log(player.isGrounded);
        if (player.isGrounded)
        {
            fallVelocity = -gravity * Time.deltaTime;
            if (jumpInput){
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
