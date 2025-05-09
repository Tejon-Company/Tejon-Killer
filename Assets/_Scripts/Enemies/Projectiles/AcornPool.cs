using System.Collections.Generic;
using UnityEngine;

public class AcornPool : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private int poolSize = 10;

    [SerializeField]
    private float projectileSpeed = 10f;

    private Queue<GameObject> pool;

    private void Awake()
    {
        pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    public GameObject GetProjectile(Vector3 position, Vector3 direction)
    {
        if (pool.Count > 0)
        {
            GameObject proj = pool.Dequeue();
            return LaunchProjectile(proj, position, direction);
        }
        else
        {
            Debug.LogWarning("No hay proyectiles disponibles en el pool.");
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

    public void ReturnProjectile(GameObject proj)
    {
        proj.SetActive(false);

        pool.Enqueue(proj);
    }
}
