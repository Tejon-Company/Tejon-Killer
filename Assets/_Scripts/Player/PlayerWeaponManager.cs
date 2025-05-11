using System.Collections.Generic;
using UnityEngine;

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

    private readonly WeaponController[] weaponSlots = new WeaponController[5];
    public int ActiveWeaponIndex { get; private set; } = -1;

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

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null)
                continue;

            var weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
            weaponInstance.gameObject.SetActive(false);
            SetupSway(weaponInstance);
            weaponSlots[i] = weaponInstance;
            return;
        }

        Debug.LogWarning("No hay espacio para más armas.");
    }

    private void SwitchWeaponToIndex(int index)
    {
        if (!IsValidSlot(index) || index == ActiveWeaponIndex)
            return;

        DeactivateCurrentWeapon();

        var newWeapon = weaponSlots[index];
        if (newWeapon == null)
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

        var currentWeapon = weaponSlots[ActiveWeaponIndex];
        if (currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);
    }

    private void ActivateWeapon(WeaponController weapon, int index)
    {
        weapon.gameObject.SetActive(true);
        SetupSway(weapon);
        ActiveWeaponIndex = index;
        EventManager.current.NewGunEvent.Invoke();
    }

    private void SetupSway(WeaponController weapon)
    {
        var sway = weapon.GetComponent<GunAnimations>();
        if (sway != null)
        {
            sway.SetPlayerController(playerController);
            playerController.SetWeaponSway(sway);
        }
    }

    private bool IsValidSlot(int index)
    {
        return index >= 0 && index < weaponSlots.Length;
    }

    public WeaponController GetActiveWeapon()
    {
        return IsValidSlot(ActiveWeaponIndex) ? weaponSlots[ActiveWeaponIndex] : null;
    }
}
