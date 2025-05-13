using System.Collections.Generic;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField]
        private PlayerController playerController;

        [Header("Configuración de armas")]
        [SerializeField]
        private List<WeaponController> startingWeapons = new();

        [SerializeField]
        private Transform weaponParentSocket;

        [SerializeField]
        private Transform defaultWeaponPosition;

        [SerializeField]
        private Transform aimingPosition;

        private readonly WeaponController[] _weaponSlots = new WeaponController[5];
        private int ActiveWeaponIndex { get; set; } = -1;

        private void Start()
        {
            foreach (var weapon in startingWeapons)
                AddWeaponToSlot(weapon);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SwitchWeaponToIndex(0);
        }

        private void AddWeaponToSlot(WeaponController weaponPrefab)
        {
            weaponParentSocket.position = defaultWeaponPosition.position;

            for (var i = 0; i < _weaponSlots.Length; i++)
            {
                if (_weaponSlots[i] is not null)
                    continue;

            var weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            weaponInstance.gameObject.SetActive(false);
            SetupWeaponGunAnimations(weaponInstance);
            _weaponSlots[i] = weaponInstance;
            return;
        }

            Debug.LogWarning("No hay espacio para más armas.");
        }

        private void SwitchWeaponToIndex(int index)
        {
            if (!IsValidSlot(index) || index == ActiveWeaponIndex)
                return;

            DeactivateCurrentWeapon();

            var newWeapon = _weaponSlots[index];
            if (newWeapon is null)
            {
                Debug.LogWarning($"No hay arma en el slot {index}");
                return;
            }

            ActivateWeapon(newWeapon, index);
        }

        private void DeactivateCurrentWeapon()
        {
            if (!IsValidSlot(ActiveWeaponIndex))
                return;

            var currentWeapon = _weaponSlots[ActiveWeaponIndex];
            currentWeapon?.gameObject.SetActive(false);
        }

    private void ActivateWeapon(WeaponController weapon, int index)
    {
        weapon.gameObject.SetActive(true);
        SetupWeaponGunAnimations(weapon);
        ActiveWeaponIndex = index;
        EventManager.current.NewGunEvent.Invoke();
    }

    private void SetupWeaponGunAnimations(WeaponController weapon)
    {
        var gunAnimations = weapon.GetComponent<GunAnimations>();
        if (gunAnimations != null)
        {
            gunAnimations.SetPlayerController(playerController);
            playerController.SetWeaponGunAnimations(gunAnimations);
        }
    }

        private bool IsValidSlot(int index)
        {
            return index >= 0 && index < _weaponSlots.Length;
        }

        public WeaponController GetActiveWeapon()
        {
            return IsValidSlot(ActiveWeaponIndex) ? _weaponSlots[ActiveWeaponIndex] : null;
        }
    }
}
