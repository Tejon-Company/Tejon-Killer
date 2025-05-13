using UnityEngine;

namespace _Scripts.Enemies.Squirrel
{
    public class Squirrel :  Enemy
    {
        [Header("References")]
        [SerializeField]
        private ProjectilesPool.ProjectilesPool acornPool;

        [SerializeField]
        private Transform firePoint;

        [Header("Shooting Variables")]
        [SerializeField]
        private float fireRate = 2f;

        [SerializeField]
        private float minVerticalLaunch = 0.1f;

        [SerializeField]
        private float maxVerticalLaunch = 1.0f;
        
        private float lastShotTime = Mathf.NegativeInfinity;

        private protected override void FindReferences()
        {
            acornPool ??= FindFirstObjectByType<ProjectilesPool.ProjectilesPool>();
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private protected override void Attack()
        {
            if (!acornPool || !Player || !firePoint)
                return;

            var targetPosition = Player.TryGetComponent(out Collider playerCollider)
                ? playerCollider.bounds.center
                : Player.position + Vector3.up;

            LaunchProjectile(targetPosition);
        }

        private void Update()
        {
            if (!Player)
                return;

            var playerIsInRange =
                Vector3.Distance(Player.position, transform.position) <= detectionRange;
            var canShoot = Time.time - lastShotTime >= fireRate;

            if (!playerIsInRange || !canShoot)
                return;

            RotateToPlayer();
            Attack();
            lastShotTime = Time.time;
        }

        private void RotateToPlayer()
        {
            var direction = Player.position - transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);
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
            ProjectilesPool.ProjectilesPool.LaunchProjectile(projectile, direction);

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
