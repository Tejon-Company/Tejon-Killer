using UnityEngine;

public class Acorn : MonoBehaviour
{
    [Header("Variables de disparo")]
    [SerializeField]
    private float lifetimeAfterHit = 0.3f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float gravityMultiplier = 2f;

    private Rigidbody rb;

    private ProjectilesPool pool;

    public void Init(ProjectilesPool poolRef)
    {
        pool = poolRef;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
            PlayerHealth player = collision.collider.GetComponentInParent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Invoke(nameof(Deactivate), 0);
        }
        else
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
