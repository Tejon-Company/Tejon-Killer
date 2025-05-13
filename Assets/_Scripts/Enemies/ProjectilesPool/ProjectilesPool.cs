using System.Collections.Generic;
using UnityEngine;

public class ProjectilesPool : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private int poolSize = 2;

    private List<GameObject> pool;

    private void Awake()
    {
        pool = new List<GameObject>();
        GameObject tmp;

        for (int i = 0; i < poolSize; i++)
        {
            tmp = Instantiate(projectilePrefab);
            tmp.SetActive(false);
            pool.Add(tmp);
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        return null;
    }

    public GameObject GetProjectile(Transform firePoint)
    {
        GameObject proj = GetPooledObject();

        if (proj == null)
            return null;

        proj.transform.position = firePoint.position;
        proj.transform.rotation = firePoint.rotation;

        proj.SetActive(true);
        return proj;
    }

    public void LaunchProjectile(GameObject projectile, Vector3 direction)
    {
        if (projectile == null)
            return;

        if (projectile.TryGetComponent(out Acorn acorn))
        {
            acorn.Launch(direction);
        }
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
    }
}
