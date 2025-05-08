using System.Collections.Generic;
using UnityEngine;

public class AcornPool : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float projectileSpeed = 10f;
    private List<GameObject> pool;


    private void Awake()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            pool.Add(proj);
        }
    }

    public GameObject GetProjectile(Vector3 position, Vector3 direction)
    {
        foreach (GameObject proj in pool)
        {
            if (!proj.activeInHierarchy)
            {
                proj.transform.position = position;
                proj.transform.rotation = Quaternion.LookRotation(direction);

                Rigidbody rb = proj.GetComponent<Rigidbody>();
                rb.linearVelocity = direction.normalized * projectileSpeed; 

                return proj;
            }
        }
        return null; 
    }
}
