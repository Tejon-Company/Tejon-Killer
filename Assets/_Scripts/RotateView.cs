using UnityEngine;

public class RotateView : MonoBehaviour
{
    private float mouseX, mouseY;
    [SerializeField, Range(0, 1000)]
    private float sensibility = 100f;

    private float rotationX, rotacionY;
    public Transform player;
    public Vector3 offset = new Vector3(0, 0.35f, 0);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Movimiento del ratón
        mouseX = Input.GetAxis("Mouse X") * sensibility * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensibility * Time.deltaTime;

        rotationX += mouseY;
        rotacionY +=mouseX;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, rotacionY, 0f);

        player.Rotate(Vector3.up * mouseX);

        transform.position = player.position + offset;

        // Liberar cursor con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
