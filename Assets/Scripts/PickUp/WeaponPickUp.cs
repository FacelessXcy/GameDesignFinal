using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponPickUp : MonoBehaviour
{
    public Weapon weapon;
    public int ammoAmount;
    private PickUpItem _pickUpItem;
    private void Start()
    {
        _pickUpItem = GetComponent<PickUpItem>();
        _pickUpItem.onPick = OnPickUp;
    }

    private void OnPickUp(PlayerWeaponController playerWeaponController)
    {
        Debug.Log("捡起；"+weapon.weaponName+"  枪内有子弹:"+ammoAmount+"发");
        playerWeaponController.AddWeapon(weapon,false,ammoAmount);
    }

}
