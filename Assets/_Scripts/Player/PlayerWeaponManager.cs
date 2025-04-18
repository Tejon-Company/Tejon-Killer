// PlayerWeaponManager.cs
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Referencia al PlayerController (arrastrar en Inspector)")]
    [SerializeField] private PlayerController playerController;

    [Header("Configuración de armas")]
    public List<WeaponController> startingWeapons = new List<WeaponController>();
    public Transform weaponParentSocket;
    public Transform defaultWeaponPosition;
    public Transform aimingPosition;

    private WeaponController[] weaponslots = new WeaponController[5];
    public int activeWeaponIndex { get; private set; } = -1;

    private void Start()
    {
        foreach (WeaponController w in startingWeapons)
            AddWeapon(w);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);
    }

    private void AddWeapon(WeaponController prefab)
    {
        weaponParentSocket.position = defaultWeaponPosition.position;
        for (int i = 0; i < weaponslots.Length; i++)
        {
            if (weaponslots[i] == null)
            {
                var clone = Instantiate(prefab, weaponParentSocket);
                clone.gameObject.SetActive(false);
                weaponslots[i] = clone;
                return;
            }
        }
    }

    private void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weaponslots.Length) return;
        if (index == activeWeaponIndex) return;

        // (Opcional) desactivo arma anterior
        if (activeWeaponIndex >= 0)
            weaponslots[activeWeaponIndex].gameObject.SetActive(false);

        var wc = weaponslots[index];
        if (wc == null)
        {
            Debug.LogWarning($"No hay arma en el slot {index}");
            return;
        }

        wc.gameObject.SetActive(true);
        activeWeaponIndex = index;

        // ** Aquí le asigno el Sway al PlayerController **
        var sway = wc.GetComponent<Sway>();
        if (sway != null)
            playerController.SetWeaponSway(sway);

        EventManager.current.NewGunEvent.Invoke();
    }
    public WeaponController GetActiveWeapon()
    {
        if (activeWeaponIndex < 0 || activeWeaponIndex >= weaponslots.Length) 
            return null;
        return weaponslots[activeWeaponIndex];
    }
}
