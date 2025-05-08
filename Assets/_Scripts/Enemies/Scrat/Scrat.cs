using UnityEngine;
using System.Collections;

public class Scrat : MonoBehaviour
{
    [SerializeField] private AcornPool acornPool;
    [SerializeField] private Transform firePoint;

    [Header("Variables de disparo")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float minVerticalLaunch = 0.1f;
    [SerializeField] private float maxVerticalLaunch = 1.0f;
    private float lastShotTime = -Mathf.Infinity;

    [Header("Efectos")]
    [SerializeField] private float flashDuration = 0.3f;
    private Renderer[] scratRenderers;
    private Color[] originalColors;

    private Transform player;

    private void Start()
    {
        scratRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[scratRenderers.Length];

        for (int i = 0; i < scratRenderers.Length; i++)
        {
            originalColors[i] = scratRenderers[i].material.GetColor("_BaseColor");
        }

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
        float distance = Vector3.Distance(player.position, transform.position);
        float verticalLaunch = Mathf.Lerp(minVerticalLaunch, maxVerticalLaunch, distance / detectionRange);

        Vector3 dir = (player.position - firePoint.position).normalized + Vector3.up * verticalLaunch;
        GameObject projectile = acornPool.GetProjectile(firePoint.position, dir);

        if (projectile.TryGetComponent(out Collider projectileCol))
        {
            Collider[] scratColliders = GetComponentsInChildren<Collider>();

            foreach (var scratCol in scratColliders)
            {
                Physics.IgnoreCollision(projectileCol, scratCol);
            }
        }

        projectile.SetActive(true);
    }
    
    public void FlashRed()
    {
        foreach (var renderer in scratRenderers)
            renderer.material.SetColor("_BaseColor", new Color(0.6f, 0.1f, 0.1f, 1f));

        Invoke(nameof(RestoreOriginalColor), flashDuration);
    }

    private void RestoreOriginalColor()
    {
        for (int i = 0; i < scratRenderers.Length; i++)
            scratRenderers[i].material.SetColor("_BaseColor", originalColors[i]);
    }
}
