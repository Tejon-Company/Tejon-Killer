using System.Collections;
using _Scripts.Enemies;
using _Scripts.Managers;
using _Scripts.Managers.Audio;
using _Scripts.Menus;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private Transform weaponMuzzle;

    [SerializeField]
    private GunAnimations gunAnimations;

    [Header("Parámetros de disparo")]
    [SerializeField]
    private LayerMask hittableLayers;

    [SerializeField]
    private float fireRange = 200f;

    [SerializeField]
    private float fireRate = 0.2f;

    [SerializeField]
    private int shotDamage = 10;

    [Header("Munición")]
    [SerializeField]
    private int maxAmmo = 8;
    public int CurrentAmmo { get; private set; }
    public int MaxAmmo => maxAmmo;

    [Header("Recarga")]
    [SerializeField]
    private float reloadTime = 0.85f;
    private bool isReloading;

    [Header("Rabia")]
    private bool isRaging = false;
    private float rageEndTime = 0f;
    private float defaultFireRate;

    [Header("Efectos visuales")]
    [SerializeField]
    private GameObject flashEffect;

    [SerializeField]
    private GameObject tracerEffectPrefab;

    [SerializeField]
    private GameObject bulletHolePrefab;

    [SerializeField]
    private float rayEffectTime = 0.2f;

    private Transform cameraTransform;
    private float lastShotTime;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        CurrentAmmo = maxAmmo;
        defaultFireRate = fireRate;
        UpdateAmmoUI();
    }

    private void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera")?.transform;
    }

    private void OnEnable()
    {
        if (EventManager.Current != null)
            EventManager.Current.rageBerryEvent.AddListener(ApplyRage);
    }

    private void OnDisable()
    {
        if (EventManager.Current != null)
            EventManager.Current.rageBerryEvent.RemoveListener(ApplyRage);
    }

    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            return;
        }
        HandleFireInput();
        HandleReloadInput();
        HandleRageState();
    }

    private void HandleFireInput()
    {
        if (!Input.GetButtonDown("Fire") || isReloading)
            return;

        if (CurrentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Time.time >= lastShotTime + fireRate)
        {
            Shoot();
            CurrentAmmo--;
            UpdateAmmoUI();
            lastShotTime = Time.time;
        }
    }

    private void HandleReloadInput()
    {
        if (Input.GetButtonDown("Reload") && CurrentAmmo < MaxAmmo && !isReloading)
            StartCoroutine(Reload());
    }

    private void HandleRageState()
    {
        if (isRaging && Time.time >= rageEndTime)
        {
            fireRate = defaultFireRate;
            isRaging = false;
        }
    }

    private void Shoot()
    {
        SfxManager.Instance.PlaySfx(SfxManager.Instance.Shoot);
        ShowFlashEffect();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireRange, hittableLayers))
        {
            ShowBulletHole(hit);
            ShowTracerEffect(weaponMuzzle.position, hit.point);

            enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(shotDamage);
            }
        }

        gunAnimations?.ApplyRecoil();
    }

    private void ShowFlashEffect()
    {
        if (!flashEffect)
            return;

        GameObject flash = Instantiate(
            flashEffect,
            weaponMuzzle.position,
            Quaternion.LookRotation(weaponMuzzle.forward),
            transform
        );
        Destroy(flash, 1f);
    }

    private void ShowBulletHole(RaycastHit hit)
    {
        if (!bulletHolePrefab)
            return;

        GameObject hole = Instantiate(
            bulletHolePrefab,
            hit.point - hit.normal * 0.01f,
            Quaternion.LookRotation(hit.normal)
        );
        Destroy(hole, 4f);
    }

    private void ShowTracerEffect(Vector3 start, Vector3 end)
    {
        if (!tracerEffectPrefab)
            return;

        GameObject tracer = Instantiate(tracerEffectPrefab);
        LineRenderer lr = tracer.GetComponent<LineRenderer>();
        if (lr)
        {
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            StartCoroutine(FadeRay(lr, rayEffectTime));
        }
    }

    private IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;

        if (gunAnimations != null)
            gunAnimations.PlayReloadAnimation(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        SfxManager.Instance.PlaySfx(SfxManager.Instance.Reload);
        CurrentAmmo = MaxAmmo;
        UpdateAmmoUI();
        isReloading = false;
    }

    private void UpdateAmmoUI()
    {
        EventManager.Current.updateBulletsEvent.Invoke(CurrentAmmo, MaxAmmo);
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

    public void ApplyRage(
        float playerBaseSpeedMultiplier,
        float playerJumpForceMultiplier,
        float weaponFireRateMultiplier,
        float duration
    )
    {
        fireRate = defaultFireRate * weaponFireRateMultiplier;
        rageEndTime = Time.time + duration;
        CurrentAmmo = MaxAmmo;
        UpdateAmmoUI();
        isRaging = true;
    }
}
