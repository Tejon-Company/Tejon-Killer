using UnityEngine;

public class FollowPlayer: MonoBehaviour
{
    public GameObject player;
    public Vector3 offset= new Vector3(0,0.35f,0);
    private void Update()
    {

        transform.Rotate(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        transform.position =player.transform.position + offset;
    }
}
