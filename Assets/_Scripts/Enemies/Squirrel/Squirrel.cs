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
    private float launchLimit;
    private Renderer[] scratRenderers;
    private Color[] originalColors;
    private Transform player;

    private void Awake()
    {
        InitRenderers();
        FindReferences();
    }

    private void InitRenderers()
    {
        scratRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[scratRenderers.Length];

        for (int i = 0; i < scratRenderers.Length; i++)
        {
            originalColors[i] = GetRendererColor(scratRenderers[i]);
        }
    }

    private Color GetRendererColor(Renderer renderer)
    {
        Material mat = renderer.material;
        if (mat.HasProperty("_BaseColor"))
            return mat.GetColor("_BaseColor");
        else if (mat.HasProperty("_Color"))
            return mat.GetColor("_Color");
        else
            return Color.white;
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

        launchLimit = detectionRange * 0.5f; 
        verticalLaunch = (distanceToPlayer < launchLimit) ? minVerticalLaunch : maxVerticalLaunch;

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
        foreach (var renderer in scratRenderers)
        {
            SetRendererColor(renderer, flashColor);
        }

        Invoke(nameof(RestoreOriginalColor), flashDuration);
    }

    private void SetRendererColor(Renderer renderer, Color color)
    {
        Material mat = renderer.material;
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
    }

    private void RestoreOriginalColor()
    {
        for (int i = 0; i < scratRenderers.Length; i++)
        {
            SetRendererColor(scratRenderers[i], originalColors[i]);
        }
    }
}