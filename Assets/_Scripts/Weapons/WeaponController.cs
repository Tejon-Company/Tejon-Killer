using System.Collections;
using _Scripts.Audio.Managers;
using _Scripts.Enemies;
using _Scripts.Events;
using _Scripts.Menus;
using UnityEngine;

namespace _Scripts.Weapons
{
    
    /// <summary>
    /// Controla la funcionalidad del arma del jugador.
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        #region References
        [Header("References")]
        [SerializeField]
        private Transform weaponMuzzle;

        [SerializeField]
        private GunAnimations gunAnimations;
        private Transform _cameraTransform;
        #endregion

        #region Weapon Parameters
        [Header("Shooting Parameters")]
        [SerializeField]
        private LayerMask hittableLayers;

        [SerializeField]
        private float fireRange = 200f;

        [SerializeField]
        private float fireRate = 0.2f;

        [SerializeField]
        private int shotDamage = 10;
        private float _lastShotTime;
        private float _defaultFireRate;
        #endregion

        #region Ammunition
        [Header("Ammunition")]
        [SerializeField]
        private int maxAmmo = 8;
        public int CurrentAmmo { get; private set; }
        public int MaxAmmo => maxAmmo;
        #endregion

        #region Reload
        [Header("Reload")]
        [SerializeField]
        private float reloadTime = 0.85f;
        private bool _isReloading;
        #endregion

        #region Rage Mode
        [Header("Rage")]
        private bool _isRaging;
        private float _rageEndTime;
        #endregion

        #region Visual Effects
        [Header("Visual Effects")]
        [SerializeField]
        private GameObject flashEffect;

        [SerializeField]
        private GameObject tracerEffectPrefab;

        [SerializeField]
        private GameObject bulletHolePrefab;

        [SerializeField]
        private float rayEffectTime = 0.2f;
        #endregion

        private void Awake()
        {
            CurrentAmmo = maxAmmo;
            _defaultFireRate = fireRate;
            UpdateAmmoUI();
        }

        private void Start()
        {
            _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera")?.transform;
        }

        private void OnEnable() => EventManager.Instance?.rageBerryEvent.AddListener(ApplyRage);

        private void OnDisable() => EventManager.Instance?.rageBerryEvent.RemoveListener(ApplyRage);

        private void Update()
        {
            if (PauseMenu.IsPaused)
                return;

            HandleFireInput();
            HandleReloadInput();
            HandleRageState();
        }

        #region Input Handling
        private void HandleFireInput()
        {
            if (!Input.GetButtonDown("Fire") || _isReloading)
                return;

            if (CurrentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            if (Time.time < _lastShotTime + fireRate)
                return;

            Shoot();
            CurrentAmmo--;
            UpdateAmmoUI();
            _lastShotTime = Time.time;
        }

        private void HandleReloadInput()
        {
            if (Input.GetButtonDown("Reload") && CurrentAmmo < MaxAmmo && !_isReloading)
                StartCoroutine(Reload());
        }
        #endregion

        #region Weapon Actions
        private void Shoot()
        {
            SfxManager.Instance.PlaySfx(SfxManager.Instance.Shoot);
            ShowFlashEffect();

            var ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            if (Physics.Raycast(ray, out var hit, fireRange, hittableLayers))
                HandleHit(hit);

            gunAnimations?.ApplyRecoil();
        }

        private void HandleHit(RaycastHit hit)
        {
            ShowBulletHole(hit);
            ShowTracerEffect(weaponMuzzle.position, hit.point);

            var enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
            enemyHealth?.TakeDamage(shotDamage);
        }

        private IEnumerator Reload()
        {
            if (_isReloading)
                yield break;

            _isReloading = true;
            gunAnimations?.PlayReloadAnimation(reloadTime);
            yield return new WaitForSeconds(reloadTime);

            SfxManager.Instance.PlaySfx(SfxManager.Instance.Reload);
            CurrentAmmo = MaxAmmo;
            UpdateAmmoUI();
            _isReloading = false;
        }
        #endregion

        #region Visual Effects
        private void ShowFlashEffect()
        {
            if (!flashEffect)
                return;

            var flash = Instantiate(
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

            var hole = Instantiate(
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

            var tracer = Instantiate(tracerEffectPrefab);
            var lr = tracer.GetComponent<LineRenderer>();
            if (!lr)
                return;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            StartCoroutine(FadeRay(lr, rayEffectTime));
        }

        private static IEnumerator FadeRay(LineRenderer lr, float duration)
        {
            var time = 0f;
            var startWidth = lr.startWidth;

            while (time < duration)
            {
                var t = time / duration;
                var width = Mathf.Lerp(startWidth, 0f, t);
                lr.startWidth = width;
                lr.endWidth = width;
                time += Time.deltaTime;
                yield return null;
            }

            Destroy(lr.gameObject);
        }
        #endregion

        #region State Management
        private void HandleRageState()
        {
            if (!_isRaging || Time.time < _rageEndTime)
                return;

            fireRate = _defaultFireRate;
            _isRaging = false;
        }

        private void UpdateAmmoUI() =>
            EventManager.Instance.updateBulletsEvent.Invoke(CurrentAmmo, MaxAmmo);

        private void ApplyRage(
            float playerBaseSpeedMultiplier,
            float playerJumpForceMultiplier,
            float weaponFireRateMultiplier,
            float duration
        )
        {
            fireRate = _defaultFireRate * weaponFireRateMultiplier;
            _rageEndTime = Time.time + duration;
            CurrentAmmo = MaxAmmo;
            UpdateAmmoUI();
            _isRaging = true;
        }
        #endregion
    }
}
