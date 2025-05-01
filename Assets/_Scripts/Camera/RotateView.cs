using UnityEngine;

public class RotateView : MonoBehaviour
{
    [SerializeField]
    private Vector2 sensibility = new Vector2(3f, 3f);

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private float maxTiltAngle = 5f;
    [SerializeField]
    private float tiltSpeed = 5f;

    private float verticalRotation = 0f;
    private float targetTilt = 0f;
    private float currentTilt = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleCameraTilt();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibility.x;
        float mouseY = Input.GetAxis("Mouse Y") * sensibility.y;

        player.Rotate(Vector3.up * mouseX);

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(verticalRotation, player.eulerAngles.y, currentTilt);
    }

    private void HandleCameraTilt()
    {
        if (playerController.sliding)
        {
            float slideX = Input.GetAxis("Horizontal");

            if (slideX > 0.1f)
                targetTilt = -maxTiltAngle;
            else if (slideX < -0.1f)
                targetTilt = maxTiltAngle;
            else
                targetTilt = 0f;
        }
        else
        {
            targetTilt = 0f;
        }

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
    }

}
