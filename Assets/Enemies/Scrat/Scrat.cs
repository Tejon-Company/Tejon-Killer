using UnityEngine;

public class Scrat : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private Transform firePoint;      
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float projectileSpeed = 15f;

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

        if (distanceToPlayer <= detectionRange)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z)); 

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }

        fireCooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}
