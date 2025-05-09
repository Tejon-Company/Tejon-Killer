using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectilesPool : MonoBehaviour
{
    [SerializeField]
    protected GameObject projectilePrefab;

    [SerializeField]
    protected int poolSize = 10;

    protected Queue<GameObject> pool;

    protected virtual void Awake()
    {
        pool = new Queue<GameObject>();
        CreatePool();
    }

    protected abstract void CreatePool();

    public abstract void Shoot(Vector3 position, Vector3 direction);
}
