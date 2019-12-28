using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPickUp : MonoBehaviour
{
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
            _pickUpSubManager.PickUpBuilding();
        }
       BuildingSystem.Instance.AddBuildingAmount(1);
    }
}
