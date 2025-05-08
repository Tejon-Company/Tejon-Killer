using UnityEngine;

public class Scrat : MonoBehaviour
{
    [SerializeField] private AcornPool acornPool;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float verticalLaunch = 0.5f;

    private float lastShotTime = -Mathf.Infinity;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (acornPool == null)
        {
            acornPool = FindFirstObjectByType<AcornPool>();
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        bool playerInRange = distance <= detectionRange;
        bool canShoot = Time.time - lastShotTime >= fireRate;

        if (playerInRange)
        {
            RotateToPlayer();
            if (canShoot)
            {
                Shoot();
                lastShotTime = Time.time;
            }
        }
    }

    private void RotateToPlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void Shoot()
    {
        Vector3 dir = (player.position - firePoint.position).normalized + Vector3.up * verticalLaunch;
        acornPool.GetProjectile(firePoint.position, dir);
    }
}
