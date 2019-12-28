using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPickUp : MonoBehaviour
{
    private PickUpItem _pickUpItem;
    private void Start()
    {
        _pickUpItem = GetComponent<PickUpItem>();
        _pickUpItem.onPick = OnPickUp;
    }

    private void OnPickUp(PlayerWeaponController playerWeaponController)
    {
       BuildingSystem.Instance.AddBuildingAmount(1);
    }
}
