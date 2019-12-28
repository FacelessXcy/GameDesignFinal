using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthPickUp : MonoBehaviour
{
    public int healAmount; 
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
        if (_pickUpSubManager!=null)
        {
            _pickUpSubManager.PickUpHealth();
        }
        playerWeaponController.GetComponent<Health>().Heal(healAmount);
    }
}
