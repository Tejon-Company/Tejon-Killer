using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private Transform weaponMuzzle;

    [SerializeField]
    private GunAnimations sway;

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

    private void Awake()
    {
        CurrentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    private void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera")?.transform;
    }

    private void Update()
    {
        HandleFireInput();
        HandleReloadInput();
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

    private void Shoot()
    {
        ShowFlashEffect();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireRange, hittableLayers))
        {
            ShowBulletHole(hit);
            ShowTracerEffect(weaponMuzzle.position, hit.point);
            /*
                        EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(shotDamage);
                        }*/
        }

        sway?.ApplyRecoil();
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

        if (sway != null)
            sway.PlayReloadAnimation(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        CurrentAmmo = MaxAmmo;
        UpdateAmmoUI();
        isReloading = false;
    }

    private void UpdateAmmoUI()
    {
        EventManager.current.updateBulletsEvent.Invoke(CurrentAmmo, MaxAmmo);
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
}
