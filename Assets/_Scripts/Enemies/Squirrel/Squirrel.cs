using System.Collections;
using UnityEngine;

public class Squirrel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private ProjectilesPool acornPool;

    [SerializeField]
    private Transform firePoint;

    [Header("Variables de disparo")]
    [SerializeField]
    private float fireRate = 2f;

    [SerializeField]
    private float detectionRange = 20f;

    [SerializeField]
    private float minVerticalLaunch = 0.1f;

    [SerializeField]
    private float maxVerticalLaunch = 1.0f;

    [Header("Efectos")]
    [SerializeField]
    private float flashDuration = 0.3f;

    [SerializeField]
    private Color flashColor = new Color(0.6f, 0.1f, 0.1f, 1f);

    private float lastShotTime = Mathf.NegativeInfinity;
    private float distanceToPlayer;
    private float verticalLaunch;
    private Renderer[] scratRenderers;
    private Color[] originalColors;
    private Transform player;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        InitRenderers();
        FindReferences();
    }

    private void InitRenderers()
    {
        scratRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[scratRenderers.Length];
        propBlock = new MaterialPropertyBlock();

        for (int i = 0; i < scratRenderers.Length; i++)
        {
            scratRenderers[i].GetPropertyBlock(propBlock);
            originalColors[i] = propBlock.GetColor("_BaseColor");
        }
    }

    private void FindReferences()
    {
        if (acornPool == null)
        {
            acornPool = FindFirstObjectByType<ProjectilesPool>();
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= detectionRange && Time.time - lastShotTime >= fireRate)
        {
            RotateToPlayer();
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void RotateToPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void Shoot()
    {
        if (acornPool == null || player == null || firePoint == null)
            return;

        Vector3 targetPosition = GetTargetPosition();
        LaunchAt(targetPosition);
    }

    private Vector3 GetTargetPosition()
    {
        if (player.TryGetComponent(out Collider playerCollider))
        {
            return playerCollider.bounds.center;
        }
        else
        {
            return player.position + Vector3.up * 1.0f;
        }
    }

    private void LaunchAt(Vector3 targetPosition)
    {
        distanceToPlayer = Vector3.Distance(targetPosition, transform.position);

        float launchThreshold = detectionRange * 0.5f; 
        verticalLaunch = (distanceToPlayer < launchThreshold) ? minVerticalLaunch : maxVerticalLaunch;

        GameObject projectile = acornPool.GetProjectile(firePoint);
        if (projectile == null)
            return;

        Vector3 toTarget = targetPosition - firePoint.position;
        Vector3 direction = new Vector3(toTarget.x, verticalLaunch, toTarget.z).normalized;

        acornPool.LaunchProjectile(projectile, direction);

        IgnoreSelfCollisions(projectile);
    }

    private void IgnoreSelfCollisions(GameObject other)
    {
        if (!other.TryGetComponent(out Collider otherCol))
            return;

        Collider[] ownColliders = GetComponentsInChildren<Collider>();
        foreach (var col in ownColliders)
        {
            Physics.IgnoreCollision(otherCol, col);
        }
    }

    public void FlashRed()
    {
        for (int i = 0; i < scratRenderers.Length; i++)
        {
            scratRenderers[i].GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", flashColor);
            scratRenderers[i].SetPropertyBlock(propBlock);
        }

        Invoke(nameof(RestoreOriginalColor), flashDuration);
    }

    private void RestoreOriginalColor()
    {
        for (int i = 0; i < scratRenderers.Length; i++)
        {
            scratRenderers[i].GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", originalColors[i]);
            scratRenderers[i].SetPropertyBlock(propBlock);
        }
    }
}