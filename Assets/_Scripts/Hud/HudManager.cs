using _Scripts.Managers;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Hud
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject weaponInfoPrefab;

        [SerializeField]
        private PlayerWeaponManager playerWeaponManager;

        private void Start()
        {
            EventManager.Current.newGunEvent.AddListener(CreateWeaponInfo);
        }

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
