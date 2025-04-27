using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour, RageInterface
{
    [Header("References")]
    public Transform weaponMuzzle;

    [Header("General")]
    public LayerMask hittableLayers;
    public GameObject bulletHolePrefab;

    [Header("Shoot Parameters")]
    public float fireRange = 200;
    public float fireRate = 0.2f;

    [Header("Rage Parameters")]
    private bool isRaging = false;
    private float rageEndTime = 0f;
    private float defaultFireRate;

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
    public GameObject tracerEffectPrefab;
    [SerializeField] float rayEffectTime= 0.2f;
    private Transform cameraPlayerTransform;
    private float lastShotTime = 0f;

    void Awake()
    {
        currentAmmo = maxAmmo;
        defaultFireRate = fireRate; 
        EventManager.current.updateBulletsEvent.Invoke(currentAmmo, maxAmmo);
    }

    private void Start()
    {
        cameraPlayerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void OnEnable()
    {
        if (EventManager.current != null)
            EventManager.current.rageBerryEvent.AddListener(ApplyRage);
    }

    private void OnDisable()
    {
        if (EventManager.current != null)
            EventManager.current.rageBerryEvent.RemoveListener(ApplyRage);
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

        // Controlar si está en modo rabia
        if (isRaging && Time.time >= rageEndTime)
        {
            fireRate = defaultFireRate; 
            isRaging = false;
        }
    }

    private void Shoot()
    {
        // Flash
        var flashClone = Instantiate(flashEffect, weaponMuzzle.position, Quaternion.LookRotation(weaponMuzzle.forward), transform);
        Destroy(flashClone, 1f);

        Ray ray = new Ray(cameraPlayerTransform.position, cameraPlayerTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireRange, hittableLayers))
        {
            // Crear bullet hole
            var hole = Instantiate(bulletHolePrefab,
                                hit.point - hit.normal * 0.01f,
                                Quaternion.LookRotation(hit.normal));
            Destroy(hole, 4f);

            // Mostrar rayo visual
            GameObject rayInstance = Instantiate(tracerEffectPrefab);
            LineRenderer lr = rayInstance.GetComponent<LineRenderer>();
            lr.SetPosition(0, weaponMuzzle.position);
            lr.SetPosition(1, hit.point);

            // Iniciar la rutina para estrechar el rayo
            StartCoroutine(FadeRay(lr, rayEffectTime)); // dura 0.1 segundos
        }

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

    private IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        GameObject tracer = Instantiate(tracerEffectPrefab);
        LineRenderer lr = tracer.GetComponent<LineRenderer>();

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        yield return null; // 1 frame
        Destroy(tracer,0.1f);
    }

    private IEnumerator FadeRay(LineRenderer lr, float duration)
    {
        float time = 0f;
        float startWidth = lr.startWidth;

        while (time < duration)
        {
            float t = time / duration;
            float width = Mathf.Lerp(startWidth, 0f, t);
            lr.startWidth = width;
            lr.endWidth = width;
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(lr.gameObject);
    }

    public void ApplyRage(float playerSpeedMultiplier, float weaponFireRateMultiplier, float duration)
    {
        fireRate = defaultFireRate * weaponFireRateMultiplier;
        rageEndTime = Time.time + duration;
        isRaging = true;
    }
}
