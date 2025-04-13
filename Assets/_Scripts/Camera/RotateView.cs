using UnityEngine;

public class RotateView : MonoBehaviour
{
    [SerializeField]
    private Vector2 sensibility = new Vector2(3f, 3f);

    [SerializeField]
    private Transform player;

    private float verticalRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibility.x;
        float mouseY = Input.GetAxis("Mouse Y") * sensibility.y;

        player.Rotate(Vector3.up * mouseX);

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(verticalRotation, player.eulerAngles.y, 0f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
