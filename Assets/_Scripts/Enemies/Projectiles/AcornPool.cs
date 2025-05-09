using UnityEngine;

public class AcornPool : ProjectilesPool
{
    [SerializeField]
    private float projectileSpeed = 10f;

    protected override void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    public GameObject GetProjectile(Vector3 position, Vector3 direction)
    {
        GameObject proj = GetProjectileFromPool();
        return LaunchProjectile(proj, position, direction);
    }

    private GameObject GetProjectileFromPool()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            return null;
        }
    }


    private GameObject LaunchProjectile(GameObject proj, Vector3 position, Vector3 direction)
    {
        proj.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));

        if (proj.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = direction.normalized * projectileSpeed;
        }

        proj.SetActive(true);
        return proj;
    }

    public override void Shoot(Vector3 position, Vector3 direction)
    {
        GameObject proj = GetProjectile(position, direction);
        if (proj == null)
            return;
    }

    public void ReturnProjectile(GameObject proj)
    {
        proj.SetActive(false);
        pool.Enqueue(proj);
    }
}
