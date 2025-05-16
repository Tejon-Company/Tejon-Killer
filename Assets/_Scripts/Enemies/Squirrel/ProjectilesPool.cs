using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Enemies.Squirrel
{
    public class ProjectilesPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject projectilePrefab;

        [SerializeField]
        private int poolSize = 2;

        private List<GameObject> _pool;

        private void Awake()
        {
            _pool = new List<GameObject>();

            for (var i = 0; i < poolSize; i++)
            {
                var tmp = Instantiate(projectilePrefab);
                tmp.SetActive(false);
                _pool.Add(tmp);
            }
        }

        private GameObject GetInactiveProjectileFromPool()
        {
            return _pool.FirstOrDefault(obj => !obj.activeInHierarchy);
        }

        public GameObject GetProjectile(Transform firePoint)
        {
            var proj = GetInactiveProjectileFromPool();

            if (!proj)
                return null;

            proj.transform.position = firePoint.position;
            proj.transform.rotation = firePoint.rotation;

            proj.SetActive(true);
            return proj;
        }

        public static void LaunchProjectile(GameObject projectile, Vector3 direction)
        {
            if (!projectile)
                return;

            if (projectile.TryGetComponent(out Acorn acorn))
                acorn.Launch(direction);
        }
    }
}