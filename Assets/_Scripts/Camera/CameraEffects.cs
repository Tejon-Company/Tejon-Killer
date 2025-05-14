using _Scripts.Menus;
using _Scripts.Player;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField]
    private Vector2 sensibility = new(3f, 3f);

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private float maxTiltAngle = 5f;

    [SerializeField]
    private float tiltSpeed = 5f;

    [Header("Camera FOV Settings")]
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private float baseFOV = 60f;

    [SerializeField]
    private float slideFOV = 80f;

    [SerializeField]
    private float dashFOV = 95f;

    [SerializeField]
    private float fovLerpSpeed = 5f;

    private float verticalRotation;
    private float targetTilt;
    private float currentTilt;

    private void Start()
    {
        LockCursor();

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = baseFOV;
        }
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            return;
        }

        HandleMouseLook();
        HandleCameraTilt();
        UpdateFOV();
    }

    private void HandleMouseLook()
    {
        var mouseX = Input.GetAxis("Mouse X") * sensibility.x;
        var mouseY = Input.GetAxis("Mouse Y") * sensibility.y;

        player.Rotate(Vector3.up * mouseX);

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(verticalRotation, player.eulerAngles.y, currentTilt);
    }

    private void HandleCameraTilt()
    {
        targetTilt = CalculateTargetTilt();
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
    }

    private float CalculateTargetTilt()
    {
        if (!playerController.IsSliding)
            return 0f;

        var horizontalInput = Input.GetAxis("Horizontal");
        const float threshold = 0.1f;

        if (Mathf.Abs(horizontalInput) <= threshold)
            return 0f;

        var direction = Mathf.Sign(horizontalInput);
        return -direction * maxTiltAngle;
    }

    private void UpdateFOV()
    {
        if (playerCamera == null)
            return;

        float targetFOV = baseFOV;

        if (playerController.IsDashing)
        {
            targetFOV = dashFOV;
        }
        else if (playerController.IsSliding)
        {
            targetFOV = slideFOV;
        }

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * fovLerpSpeed
        );
    }
}
