using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Transform weaponMuzzle;

    [Header("General")]
    public LayerMask hittableLayers;
    public GameObject bulletHolePrefab;

    [Header("Shoot Parameters")]
    public float fireRange = 200;
    public float fireRate = 0.2f;

    [Header("AMMO")]
    [SerializeField] private int maxAmmo = 8;
    public int currentAmmo { get; private set; }
    public int MaxAmmo => maxAmmo;

    [Header("RELOAD")]
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("References")]
    [SerializeField] private Sway sway;

    [Header("SOUNDS & VISUALS")]
    public GameObject flashEffect;

    private Transform cameraPlayerTransform;
    private float lastShotTime = 0f;

    void Awake()
    {
        currentAmmo = maxAmmo;
        EventManager.current.updateBulletsEvent.Invoke(currentAmmo, maxAmmo);
    }

    private void Start()
    {
        cameraPlayerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        // 1) Disparos deshabilitados mientras recargo
        if (Input.GetButtonDown("Fire"))
        {
            if (isReloading)
                return;

            // 2) Si no hay balas, auto-recarga
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
            else
            {
                // 3) Si hay balas y no recargo, disparo
                if (Time.time > lastShotTime + fireRate)
                {
                    Shoot();
                    currentAmmo--;
                    EventManager.current.updateBulletsEvent.Invoke(currentAmmo, maxAmmo);
                    lastShotTime = Time.time;
                }
            }
        }

        // 4) Recarga manual sólo si no estoy recargando y no estoy lleno
        if (Input.GetButtonDown("Reload") && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        // Efecto de flash
        var flashClone = Instantiate(flashEffect,
                                     weaponMuzzle.position,
                                     Quaternion.LookRotation(weaponMuzzle.forward),
                                     transform);
        Destroy(flashClone, 1f);

        // Raycast de impacto
        if (Physics.Raycast(cameraPlayerTransform.position,
                            cameraPlayerTransform.forward,
                            out RaycastHit hit,
                            fireRange,
                            hittableLayers))
        {
            var hole = Instantiate(bulletHolePrefab,
                                   hit.point - hit.normal * 0.01f,
                                   Quaternion.LookRotation(hit.normal));
            Destroy(hole, 4f);
        }

        // Recoil
        sway?.ApplyRecoil();
    }

    private IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;
        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        EventManager.current.updateBulletsEvent.Invoke(currentAmmo, maxAmmo);

        isReloading = false;
        Debug.Log("¡Recargada!");
    }
}
