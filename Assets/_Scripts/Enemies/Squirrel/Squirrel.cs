using UnityEngine;

namespace _Scripts.Enemies.Squirrel
{
    public class Squirrel : Enemy
    {
        [Header("References")]
        [SerializeField]
        private ProjectilesPool acornPool;

        [SerializeField]
        private Transform firePoint;

        [Header("Shooting Variables")]
        [SerializeField]
        private float fireRate = 2f;

        [SerializeField]
        private float minVerticalLaunch = 0.1f;

        [SerializeField]
        private float maxVerticalLaunch = 1.0f;

        private float _lastShotTime = Mathf.NegativeInfinity;
        private Animator _animator;
        private static readonly int ShootTrigger = Animator.StringToHash("ShootTrigger");

        private void Start() => _animator = GetComponentInChildren<Animator>();

        private protected override void FindReferences()
        {
            acornPool ??= FindFirstObjectByType<ProjectilesPool>();
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private protected override void Attack()
        {
            if (!acornPool || !Player || !firePoint)
                return;

            var targetPosition = Player.TryGetComponent(out Collider playerCollider)
                ? playerCollider.bounds.center
                : Player.position + Vector3.up;

            _animator.SetTrigger(ShootTrigger);
            LaunchProjectile(targetPosition);
        }

        private void Update()
        {
            if (!Player)
                return;

            var playerIsInRange =
                Vector3.Distance(Player.position, transform.position) <= detectionRange;
            var canShoot = Time.time - _lastShotTime >= fireRate;

            if (!playerIsInRange || !canShoot)
                return;

            RotateToPlayer();
            Attack();
            _lastShotTime = Time.time;
        }

        private void LaunchProjectile(Vector3 targetPosition)
        {
            var verticalLaunch =
                Vector3.Distance(targetPosition, transform.position) < detectionRange * 0.5f
                    ? minVerticalLaunch
                    : maxVerticalLaunch;

            var projectile = acornPool.GetProjectile(firePoint);
            if (!projectile)
                return;

            var direction = new Vector3(
                targetPosition.x - firePoint.position.x,
                verticalLaunch,
                targetPosition.z - firePoint.position.z
            ).normalized;
            ProjectilesPool.LaunchProjectile(projectile, direction);

            IgnoreSelfCollisions(projectile);
        }

        private void IgnoreSelfCollisions(GameObject projectile)
        {
            if (!projectile.TryGetComponent(out Collider projectileCollider))
                return;

            foreach (var col in GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(projectileCollider, col);
        }
    }
}
