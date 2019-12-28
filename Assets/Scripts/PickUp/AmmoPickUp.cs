using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AmmoPickUp : MonoBehaviour
{
    public AmmoType ammoType;
    public int ammoAmount; 
    private PickUpItem _pickUpItem;
    private PickUpSubManager _pickUpSubManager;
    private void Start()
    {
        _pickUpItem = GetComponent<PickUpItem>();
        _pickUpSubManager = _pickUpItem.pickUpSubManager;
        _pickUpItem.onPick = OnPickUp;
    }
    private void OnPickUp(PlayerWeaponController playerWeaponController)
    {
        Debug.Log("捡起子弹，子弹类型；"+ammoType+" 数量："+ammoAmount);
        if (_pickUpSubManager!=null)
        {
            _pickUpSubManager.PickUpAmmo();
        }
        playerWeaponController.AddAmmoAmount(ammoType,ammoAmount);
    }
}
