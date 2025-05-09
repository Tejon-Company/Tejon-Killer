using UnityEngine;

public class Acorn : MonoBehaviour
{
    [SerializeField]
    private float lifetimeAfterHit = 0.3f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float gravityMultiplier = 2f;

    private Rigidbody rb;

    private AcornPool pool;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pool = FindFirstObjectByType<AcornPool>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(
            Vector3.down * Physics.gravity.magnitude * (gravityMultiplier - 1),
            ForceMode.Acceleration
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.collider.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Invoke(nameof(Deactivate), lifetimeAfterHit);
    }

    private void Deactivate()
    {
        if (pool != null)
        {
            pool.ReturnProjectile(gameObject);
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }
}
