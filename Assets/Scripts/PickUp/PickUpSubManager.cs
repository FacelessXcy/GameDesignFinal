using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSubManager : MonoBehaviour
{
    public float refreshTime;
    private float _currentRefreshTime;
    
    public bool spawnHealth;
    public bool spawnAmmo;
    public bool spawmWeapon;
    public bool spawnBuilding;
    
    public PickUpItem healthPickUpItem;
    public PickUpItem ammoPickUpItem;
    public PickUpItem weaponPickUpItem;
    public PickUpItem buildingPickUpItem;
    
    private Transform _healthPoint;
    private Transform _ammoPoint;
    private Transform _weaponPoint;
    private Transform _buildingPoint;

    private bool _haveAmmo;
    private bool _haveHealth;
    private bool _haveWeapon;
    private bool _haveBuilding;
    private void Start()
    {
        _healthPoint = transform.Find("HealthPoint");
        _ammoPoint = transform.Find("AmmoPoint");
        _weaponPoint = transform.Find("WeaponPoint");
        _buildingPoint = transform.Find("BuildingPoint");
    }

    private void Update()
    {
        _currentRefreshTime += Time.deltaTime;
        if (_currentRefreshTime>=refreshTime)
        {
            Refresh();
            _currentRefreshTime = 0.0f;
        }
    }

    private void Refresh()
    {
        if (!_haveAmmo)
        {
            if (spawnHealth)
            {
                  Instantiate(ammoPickUpItem, _ammoPoint.position,
                        Quaternion.identity,_ammoPoint).GetComponent<PickUpItem>()
                    .pickUpSubManager=this;
                  _haveAmmo = true;
            }
        }
        if (!_haveWeapon)
        {
            if (spawmWeapon)
            {
                Instantiate(weaponPickUpItem, _weaponPoint.position,
                        Quaternion.identity,_weaponPoint).GetComponent<PickUpItem>()
                    .pickUpSubManager=this;
                _haveWeapon = true;
            }
        }
        if (!_haveHealth)
        {
            if (spawnHealth)
            {
                Instantiate(healthPickUpItem, _healthPoint.position,
                        Quaternion.identity,_healthPoint).GetComponent<PickUpItem>()
                    .pickUpSubManager=this;
                _haveHealth = true;
            }
        }
        if (!_haveBuilding)
        {
            if (spawnBuilding)
            {
                Instantiate(buildingPickUpItem, _buildingPoint.position,
                        Quaternion.identity,_buildingPoint).GetComponent<PickUpItem>()
                    .pickUpSubManager=this;
                _haveBuilding = true;
            }
        }
    }

    public void PickUpAmmo()
    {
        _haveAmmo = false;
    }
    public void PickUpWeapon()
    {
        _haveWeapon = false;
    }
    public void PickUpHealth()
    {
        _haveHealth = false;
    }
    public void PickUpBuilding()
    {
        _haveBuilding = false;
    }

}
