using _Scripts.Events;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Hud
{
    /// <summary>
    /// Gestiona los elementos de la interfaz de usuario relacionados con las armas,
    /// creando y actualizando la información de munición.
    /// </summary>
    public class HudManager : MonoBehaviour
    {
        public GameObject weaponInfoPrefab;

        [SerializeField]
        private PlayerWeaponManager playerWeaponManager;

        private void Start() => EventManager.Instance.newGunEvent.AddListener(CreateWeaponInfo);

        private void CreateWeaponInfo()
        {
            var uiClone = Instantiate(weaponInfoPrefab, transform);

            var info = uiClone.GetComponent<WeaponInfoHud>();
            var weaponController = playerWeaponManager.GetActiveWeapon();

            if (info && weaponController)
                info.UpdateBullets(weaponController.CurrentAmmo, weaponController.MaxAmmo);
        }
    }
}
