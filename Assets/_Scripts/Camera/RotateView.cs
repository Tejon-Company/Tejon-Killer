using UnityEngine;

public class RotateView : MonoBehaviour
{
    [SerializeField]
    private Vector2 sensibility = new Vector2(3f, 3f);

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerController playerController; // Referencia al PlayerController

    [SerializeField]
    private float maxTiltAngle = 5f; // Ángulo máximo de inclinación
    [SerializeField]
    private float tiltSpeed = 5f; // Velocidad de inclinación

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
        HandleMouseLook();  // Controlar la rotación por el ratón
        HandleCameraTilt(); // Controlar la inclinación de la cámara si está deslizando
    }

    private void HandleMouseLook()
    {
        // Obtener la entrada del ratón
        float mouseX = Input.GetAxis("Mouse X") * sensibility.x;
        float mouseY = Input.GetAxis("Mouse Y") * sensibility.y;

        // Rotación horizontal (eje Y) del jugador
        player.Rotate(Vector3.up * mouseX);

        // Rotación vertical (eje X) de la cámara
        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Aplicar la rotación vertical de la cámara con la inclinación actual
        transform.rotation = Quaternion.Euler(verticalRotation, player.eulerAngles.y, currentTilt);
    }

    private void HandleCameraTilt()
    {
        // Solo inclinar la cámara si el jugador está deslizando
        if (playerController.sliding)
        {
            // Revisamos si el jugador se desliza a la izquierda o derecha usando Input.GetAxis("Horizontal")
            float slideX = Input.GetAxis("Horizontal"); // Dirección de movimiento horizontal (A/D)

            // Inclinación a la izquierda o derecha
            if (slideX > 0.1f)
                targetTilt = -maxTiltAngle; // Deslizando a la derecha
            else if (slideX < -0.1f)
                targetTilt = maxTiltAngle;  // Deslizando a la izquierda
            else
                targetTilt = 0f;  // No deslizando horizontalmente
        }
        else
        {
            targetTilt = 0f; // Sin inclinación
        }

        // Suavizar la inclinación de la cámara
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
    }

}
