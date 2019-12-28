using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName="WeaponData",menuName="Weapon Data")]
public class WeaponData:ScriptableObject
{
    [Header("位置偏移")] 
    public Vector3 offset;
    
    [Header("通用属性")]
    public string weaponName;
    public AmmoType ammoType;
    public FireType fireType;
    public WeaponType weaponType;
    public float reloadTime;
    public float fireTime;
    public int maxAmmo;
    public int damage;    
    public AudioClip fireClip;
    public AudioClip reloadClip;
    public GameObject dropWeaponPrefab;
    public Sprite crosshair;
    [Header("普通枪械专用")]
    public float range;
    public float bulletSpreadAngle;
    public GameObject bulletHole;
    public AudioClip dryFireClip;
    public GameObject cartridge;
    [Header("火箭发射器专用")]
    public bool isRPG;
    public GameObject projectilePrefab;
    [Header("火焰喷射器专用")]
    public bool isFlameThrower;
    [Header("近战武器专用")] 
    public bool isMeleeWeapon;
}

