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
    [SerializeField]private int maxAmmo=8;
    public int currentAmmo {get; private set;}

    [Header("RELOAD")]
    public float reloadTime =1.5f;
    private bool isReloading= false;

    [Header("References")]
    [SerializeField] private Sway sway;

    [Header("SOUNDS & VISUALS")]
    public GameObject flashEffect;



    private Transform cameraPlayerTransform;
    private float lastShotTime = 0f;

    void Awake()
    {
        currentAmmo =maxAmmo;
    }

    private void Start()
    {
        cameraPlayerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire")) // Comprobar si el jugador intenta disparar
        {
            if (currentAmmo <= 0) // Si no hay balas
            {
                // Si no hay balas, iniciar el proceso de recarga automáticamente
                if (!isReloading) // Evitar iniciar una recarga si ya está en curso
                {
                    StartCoroutine(Reload());
                }
            }
            else
            {
                TryShoot(); // Si hay balas, disparar
            }
            lastShotTime = Time.time; // Actualizar el tiempo del último disparo
        }
        if (Input.GetButtonDown("Reload") && currentAmmo < maxAmmo) // Recargar manualmente si no está lleno
        {
            StartCoroutine(Reload());
        }
    }

    private bool TryShoot()
    {
        if (currentAmmo >= 1 && Time.time > lastShotTime + fireRate)
        {
            Shoot();
            currentAmmo -= 1;
            return true;
        }
        return false;
    }

    private void Shoot()
    {
        GameObject flashClone =Instantiate(flashEffect, weaponMuzzle.position,Quaternion.Euler(weaponMuzzle.forward), transform);
        Destroy(flashClone,1f);
        RaycastHit hit;
        if (Physics.Raycast(cameraPlayerTransform.position, cameraPlayerTransform.forward, out hit, fireRange, hittableLayers))
        {
            GameObject bulletHoleClone = Instantiate(bulletHolePrefab, hit.point + hit.normal * -0.01f, Quaternion.LookRotation(hit.normal));
            Destroy(bulletHoleClone, 4f);
        }

        sway?.ApplyRecoil();
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break; // Si ya estamos recargando, salir de la función
        isReloading = true;
        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("¡Recargada!");
    }
}
