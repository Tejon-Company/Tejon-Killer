using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Enemies.Squirrel
{
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

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(
                Vector3.down * (Physics.gravity.magnitude * (gravityMultiplier - 1)),
                ForceMode.Acceleration
            );
        }

        public void Launch(Vector3 direction)
        {
            _rigidbody.linearVelocity = direction.normalized * speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                Invoke(nameof(Deactivate), lifetimeAfterHit);
                return;
            }

            Invoke(nameof(Deactivate), 0);
            var player = collision.collider.GetComponentInParent<PlayerHealth>();
            player?.TakeDamage(damage);
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}