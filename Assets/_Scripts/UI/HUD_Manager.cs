using UnityEngine;

public class HUD_Manager: MonoBehaviour
{
    public GameObject WeaponInfoPrefab;
    void Start()
    {
        EventManager.current.NewGunEvent.AddListener(CreateWeaponInfo);
    }

    public void CreateWeaponInfo(){
        Instantiate(WeaponInfoPrefab,transform);
    }
}
