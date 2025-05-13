using UnityEngine;

public class Acorn : MonoBehaviour
{
    [Header("Variables de disparo")]
    [SerializeField]
    private float speed = 12f;

    [SerializeField]
    private float lifetimeAfterHit = 0.3f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float gravityMultiplier = 2f;

    private Rigidbody rb;
    private ProjectilesPool pool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(ProjectilesPool poolRef)
    {
        pool = poolRef;
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(Deactivate), 0);
            PlayerHealth player = collision.collider.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        else
        {
            Invoke(nameof(Deactivate), lifetimeAfterHit);
        }
    }

    private void Deactivate()
    {
        if (pool != null)
            pool.ReturnProjectile(gameObject);
        else
            gameObject.SetActive(false);
    }
}
