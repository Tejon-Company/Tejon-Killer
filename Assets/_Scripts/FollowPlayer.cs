using UnityEngine;

public class FollowPlayer: MonoBehaviour
{
    public GameObject player;
    public Vector3 offset= new Vector3(0,0.35f,0);
    private void Update()
    {
        transform.position =player.transform.position + offset;
    }
}
