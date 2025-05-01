using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    private Vector3 offset = new Vector3(0f, 0.35f, 0f);

    private void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
    