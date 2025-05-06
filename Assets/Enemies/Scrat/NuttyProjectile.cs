using UnityEngine;

public class NuttyProjectile : MonoBehaviour
{
    [SerializeField] private float lifetimeAfterHit = 0.3f;
    [SerializeField] private int damage = 1;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.collider.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject, lifetimeAfterHit);
    }
}
