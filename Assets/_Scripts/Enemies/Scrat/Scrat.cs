using System.Collections;
using UnityEngine;

public class Scrat : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private AcornPool acornPool;

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
    private Renderer[] scratRenderers;
    private Color[] originalColors;
    private Transform player;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        scratRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[scratRenderers.Length];
        propBlock = new MaterialPropertyBlock();

        for (int i = 0; i < scratRenderers.Length; i++)
        {
            scratRenderers[i].GetPropertyBlock(propBlock);
            originalColors[i] = propBlock.GetColor("_BaseColor");
        }

        if (acornPool == null)
        {
            acornPool = FindFirstObjectByType<AcornPool>();
            if (acornPool == null)
            {
                Debug.LogError("No se encontró una instancia de AcornPool.");
            }
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("No se encontró el jugador con la etiqueta 'Player'.");
        }
    }

    private void Update()
    {
        if (player == null)
            return;

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

        float distance = Vector3.Distance(player.position, transform.position);
        float verticalLaunch = Mathf.Lerp(
            minVerticalLaunch,
            maxVerticalLaunch,
            distance / detectionRange
        );

        Vector3 direction =
            (player.position - firePoint.position).normalized + Vector3.up * verticalLaunch;

        GameObject projectile = acornPool.GetProjectile(firePoint.position, direction);

        if (projectile.TryGetComponent(out Collider projectileCol))
        {
            foreach (var col in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(projectileCol, col);
            }
        }

        projectile.SetActive(true);
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
