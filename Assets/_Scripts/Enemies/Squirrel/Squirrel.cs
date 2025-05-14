using UnityEngine;

namespace _Scripts.Enemies.Squirrel
{
    public class Squirrel : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        [Header("References")]
        private ProjectilesPool.ProjectilesPool acornPool;

        [SerializeField]
        private Transform firePoint;

        [Header("Shooting Variables")]
        [SerializeField]
        private float fireRate = 2f;

        [SerializeField]
        private float detectionRange = 20f;

        [SerializeField]
        private float minVerticalLaunch = 0.1f;

        [SerializeField]
        private float maxVerticalLaunch = 1.0f;

        [Header("Effects")]
        [SerializeField]
        private float flashDuration = 0.3f;

        [SerializeField]
        private Color flashColor = new(0.6f, 0.1f, 0.1f, 1f);

        private float _lastShotTime = Mathf.NegativeInfinity;
        private Renderer[] _renderers;
        private Color[] _originalColors;
        private Transform _player;

        private void Awake()
        {
            InitRenderers();
            FindReferences();
            
        }

        private void InitRenderers()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _originalColors = new Color[_renderers.Length];

            for (var i = 0; i < _renderers.Length; i++)
                _originalColors[i] = GetRendererColor(_renderers[i]);
        }

        private static Color GetRendererColor(Renderer renderer)
        {
            var mat = renderer.material;
            return mat.HasProperty(BaseColor) ? mat.GetColor(BaseColor)
                : mat.HasProperty(Color1) ? mat.GetColor(Color1)
                : Color.white;
        }

        private void FindReferences()
        {
            acornPool ??= FindFirstObjectByType<ProjectilesPool.ProjectilesPool>();
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (!_player)
                return;

            var playerIsInRange =
                Vector3.Distance(_player.position, transform.position) <= detectionRange;
            var canShoot = Time.time - _lastShotTime >= fireRate;

            if (!playerIsInRange || !canShoot)
                return;

            RotateToPlayer();
            Shoot();
            _lastShotTime = Time.time;
        }

        private void RotateToPlayer()
        {
            var direction = _player.position - transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        private void Shoot()
        {
            if (!acornPool || !_player || !firePoint)
                return;

            var targetPosition = _player.TryGetComponent(out Collider playerCollider)
                ? playerCollider.bounds.center
                : _player.position + Vector3.up;

            LaunchProjectile(targetPosition);
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

        public void FlashRed()
        {
            foreach (var objectRenderer in _renderers)
                SetRendererColor(objectRenderer, flashColor);

            Invoke(nameof(RestoreOriginalColors), flashDuration);
        }

        private static void SetRendererColor(Renderer objectRenderer, Color color)
        {
            var mat = objectRenderer.material;
            if (mat.HasProperty(BaseColor))
                mat.SetColor(BaseColor, color);
            else if (mat.HasProperty(Color1))
                mat.SetColor(Color1, color);
        }

        private void RestoreOriginalColors()
        {
            for (var i = 0; i < _renderers.Length; i++)
                SetRendererColor(_renderers[i], _originalColors[i]);
        }
    }
}
