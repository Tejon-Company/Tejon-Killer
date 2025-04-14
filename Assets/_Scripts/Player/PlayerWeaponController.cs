using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class PlayerWeaponController: MonoBehaviour
{
    public List<WeaponController> startingWeapons = new List<WeaponController>();

    public Transform weaponParentSocket;
    public Transform defaultWeaponPosition;
    public Transform aimingPosition;

    public int activeWeaponIndex {get; private set;}

    private WeaponController[] weaponslots = new WeaponController[5];

    void Start()
    {
        activeWeaponIndex =-1;
        foreach(WeaponController startingWeapon in startingWeapons)
        {
            AddWeapon(startingWeapon);
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            SwitchWeapon(0);
        }
    }

    private void SwitchWeapon(int p_weaponIndex){
        if(p_weaponIndex!= activeWeaponIndex && p_weaponIndex>=0){
            weaponslots[p_weaponIndex].gameObject.SetActive(true);
            activeWeaponIndex=p_weaponIndex;
        }

    }
    private void AddWeapon(WeaponController p_weaponPrefab){
        weaponParentSocket.position= defaultWeaponPosition.position;
        for (int i=0; i<weaponslots.Length;i++)
        {
            if(weaponslots[i]==null){
                WeaponController weaponClone= Instantiate(p_weaponPrefab, weaponParentSocket);
                weaponClone.gameObject.SetActive(false);

                weaponslots[i]=weaponClone;
                return;
            }
        }
    }
}