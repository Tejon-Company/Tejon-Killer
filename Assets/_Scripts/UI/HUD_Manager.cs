using UnityEngine;

public class HUD_Manager: MonoBehaviour
{
    public GameObject WeaponInfoPrefab;
    [SerializeField] private PlayerWeaponManager playerWeaponManager;
    void Start()
    {
        EventManager.current.NewGunEvent.AddListener(CreateWeaponInfo);
    }

    public void CreateWeaponInfo(){
        var uiClone = Instantiate(WeaponInfoPrefab, transform);

        var info = uiClone.GetComponent<WeaponUI_info>();
        var wc   = playerWeaponManager.GetActiveWeapon();
        if (info != null && wc != null)
            info.UpdateBullets(wc.CurrentAmmo, wc.MaxAmmo);
    }
}
