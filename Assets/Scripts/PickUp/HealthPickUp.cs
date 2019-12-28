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
    private void Start()
    {
        _pickUpItem = GetComponent<PickUpItem>();
        _pickUpItem.onPick = OnPickUp;
    }

    private void OnPickUp(PlayerWeaponController playerWeaponController)
    {
        playerWeaponController.GetComponent<Health>().Heal(healAmount);
    }
}
