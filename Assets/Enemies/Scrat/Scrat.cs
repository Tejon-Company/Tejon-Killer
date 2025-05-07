using UnityEngine;

public class Scrat : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private Transform firePoint;      
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float verticalLaunch = 0.5f;

    private float fireCooldown;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fireCooldown = 0f;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        bool playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange)
        {
            RotateToPlayer();

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }

        fireCooldown -= Time.deltaTime;
    }

    private void RotateToPlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 launchDir = firePoint.forward + firePoint.up * verticalLaunch; 
        rb.linearVelocity = launchDir.normalized * projectileSpeed;
    }
}
