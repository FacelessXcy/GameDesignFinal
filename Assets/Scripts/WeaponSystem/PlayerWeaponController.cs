using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xcy.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerWeaponController : MonoSingleton<PlayerWeaponController>
{

    public int maxAmmoAmount;
    
    public List<Weapon> startintWeapons;
    public Weapon currentWeapon;
    public AudioSource bulletAudio;
    public AudioSource flameThrowerAudio;
    public WeaponType CurrentWeaponType
    {
        get; 
        private set;
    }
    [HideInInspector]
    public bool isOpen;
    [HideInInspector]
    public bool gameIsPaused;
    [SerializeField]
    //private Weapon[] _weaponSlots=new Weapon[4];
    private Dictionary<WeaponType,Weapon> _weapons=new Dictionary<WeaponType, Weapon>();
    
    private bool _hasFired;
    public bool HasFired => _hasFired;
    public Func<GameObject> factorMethod;
    private bool _hasReloaded;
    private PlayerInput _playerInput;
    private Dictionary<AmmoType, int> _ammoDictionary=new Dictionary<AmmoType, int>();
    private Transform _dropPoint;
    private Dictionary<AmmoType, SimpleObjectPool<GameObject>>
        _cartridgePools=new Dictionary<AmmoType, SimpleObjectPool<GameObject>>();

    public Transform CurrentCartridgeSpawnpoint { get;private set; }
    public Transform CartridgeHandle{ get;private set; }

    private void Start()
    {
        CartridgeHandle= GameObject.FindWithTag("CartridgeHandle").transform;
        bulletAudio = transform.Find("BulletAudio")
            .GetComponent<AudioSource>();
        _dropPoint = Camera.main.transform.Find("DropWeaponPoint");
        flameThrowerAudio = transform.Find("FlameThrowerAudio")
            .GetComponent<AudioSource>();
        InitWeaponDictionary();
        _playerInput = PlayerInput.Instance;
        foreach (Weapon weapon in startintWeapons)
        {
            if (weapon!=null)
            {
                Debug.Log("Add"+weapon.weaponName);
                AddWeapon(weapon,true,-1);
            }
        }
        SwitchWeapon(WeaponType.Melee);
        AddAmmoAmount(AmmoType.PistolAmmo, 11);
    }


    private void Update()
    {
        if (!isOpen||gameIsPaused)
        {
            bulletAudio.mute = true;
            flameThrowerAudio.mute = true;
            return;
        }
        bulletAudio.mute = false;
        flameThrowerAudio.mute = false;
        Debug.DrawRay(transform.position,transform.forward.normalized*10.0f);
        currentWeapon = GetCurrentWeapon();
        if (currentWeapon!=null)
        {
            _hasFired = currentWeapon.ShootInput(_playerInput.leftFireDown,
                _playerInput.leftFireHold, _playerInput.leftFireUp);
            _hasReloaded =
                currentWeapon.ReloadInput(_playerInput.reload);
            CheckChangeWeapon();

        }
    }

    private void InitWeaponDictionary()
    {
        _weapons.Add(WeaponType.Melee,null);
        _weapons.Add(WeaponType.Pistol,null);
        _weapons.Add(WeaponType.SimpleWeapon, null);
        _weapons.Add(WeaponType.SpecialWeapon,null);
    }

    private void CheckChangeWeapon()
    {
        if (PlayerInput.Instance.changeWeapon1)
        {
            SwitchWeapon(WeaponType.Melee);
        }else if (PlayerInput.Instance.changeWeapon2)
        {
            SwitchWeapon(WeaponType.Pistol);
        }else if (PlayerInput.Instance.changeWeapon3)
        {
            SwitchWeapon(WeaponType.SimpleWeapon);
        }else if (PlayerInput.Instance.changeWeapon4)
        {
            SwitchWeapon(WeaponType.SpecialWeapon);
        }

        if (!currentWeapon.isFlameThrower&&
            !currentWeapon.isMeleeWeapon&&
            !currentWeapon.isRPG)
        {
            CurrentCartridgeSpawnpoint = (currentWeapon.WeaponComponent
                as WeaponComponentSimpleGun).CartridgeSpawnpoint;
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return GetWeaponByWeaponType(CurrentWeaponType);
    }

    public Weapon GetWeaponByWeaponType(WeaponType weaponType)
    {
        return _weapons[weaponType];
    }


    public bool AddWeapon(Weapon weapon,bool isInit,int ammoInWeapon)
    {
        if (HasThisWeapon(weapon))
        {
            //Debug.Log("Has This Weapon:"+weapon.name);
            return false;
        }
        //Debug.Log("Not has This Weapon:"+weapon.weaponName);
        // 替换同一种枪械
        Weapon tempWeapon = null;
        if (!isInit)
        {
            tempWeapon = Instantiate(weapon, Camera.main.transform.position,
                Camera.main.transform.rotation, Camera.main.transform);
        }
        else
        {
            tempWeapon = weapon;
        }
        tempWeapon.owner = this;
        RemoveWeapon(HasThisWeaponType(tempWeapon));
        _weapons[tempWeapon.weaponType] = tempWeapon;
        if (ammoInWeapon==-1)
        {
            tempWeapon.CurAmmo = tempWeapon.maxAmmo;
        }
        else
        {
            tempWeapon.CurAmmo = ammoInWeapon;
        }
        tempWeapon.gameObject.SetActive(false);
        
        return true;
    }

    public bool RemoveWeapon(Weapon weapon)
    {
        if (weapon==currentWeapon)
        {
            SwitchWeapon(WeaponType.Melee);
        }
        if (weapon==null)
        {
            return false;
        }
        if (HasThisWeapon(weapon))
        {
            
            GameObject dropGo= Instantiate(_weapons[weapon.weaponType]
            .dropWeaponPrefab, _dropPoint.position, Quaternion.LookRotation
                (-_dropPoint.right,Vector3.up));
            dropGo.GetComponent<Rigidbody>().AddForce(_dropPoint.forward*300.0f);
            dropGo.GetComponent<WeaponPickUp>()
                .ammoAmount = _weapons[weapon.weaponType].CurAmmo;
            Destroy(_weapons[weapon.weaponType].gameObject);
            _weapons[weapon.weaponType] = null;
            return true;
        }
        return false;
    }

    public Weapon HasThisWeaponType(Weapon weapon)
    {
        return HasThisWeaponType(weapon.weaponType);
    }

    public Weapon HasThisWeaponType(WeaponType weaponType)
    {
        return _weapons[weaponType];
    }

    public bool HasThisWeapon(Weapon weapon)
    {
        if (_weapons[weapon.weaponType]!=null&&
            _weapons[weapon.weaponType].weaponName==weapon.weaponName)
        {
            return true;
        }
        return false;
    }


    public int GetAmmoAmount(AmmoType ammoType)
    {
        if (!_ammoDictionary.ContainsKey(ammoType))
        {
            return 0;
        }

        return _ammoDictionary[ammoType];
    }

    //返回值为 是否完全按照Count值添加子弹
    public bool AddAmmoAmount(AmmoType ammoType,int count)
    {
        if (!_ammoDictionary.ContainsKey(ammoType))
        {
            if (count>maxAmmoAmount)
            {
                count = maxAmmoAmount;
            }
            ChangeAmmoAmount(ammoType,count);
            return false;
        }

        if (_ammoDictionary[ammoType]+count>maxAmmoAmount)
        {
            ChangeAmmoAmount(ammoType,maxAmmoAmount);
            return false;
        }
        ChangeAmmoAmount(ammoType,_ammoDictionary[ammoType]+count);
        return true;
    }

    //消耗子弹
    public bool RemoveAmmoAmount(AmmoType ammoType,int count)
    {
        if (!_ammoDictionary.ContainsKey(ammoType))
        {
            count = 0;
            ChangeAmmoAmount(ammoType,count);
            return false;
        }
        if (_ammoDictionary[ammoType]-count<0)
        {
            ChangeAmmoAmount(ammoType,0);
            return false;
        }
        ChangeAmmoAmount(ammoType,_ammoDictionary[ammoType]-count);
        return true;
    }

    public void ChangeAmmoAmount(AmmoType ammoType,int count)
    {
        if (!_ammoDictionary.ContainsKey(ammoType))
        {
            _ammoDictionary.Add(ammoType,0);
        }
        _ammoDictionary[ammoType] = count;
    }

    public bool SwitchWeapon(WeaponType weaponType)
    {
        if (currentWeapon!=null)
        {
            if (currentWeapon.weaponType==weaponType)
            {
                return false;
            }
        }
        if (HasThisWeaponType(weaponType)!=null)
        {
            if (currentWeapon!=null)
            {
                currentWeapon.gameObject.SetActive(false);
            }

            CurrentWeaponType = weaponType;
            currentWeapon = GetCurrentWeapon();
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.DrawWeapon();
            return true;
        }
        return false;
    }
    

    public void OpenWeaponSystem()
    {
        isOpen = true;
    }

    public void CloseWeaponSystem()
    {
        isOpen = false;
    }

    
    public void AllocateCartridge(AmmoType ammoType,Vector3 position,
        Quaternion rotation,float cartridgeLiveTime)
    {
        if (!_cartridgePools.ContainsKey(ammoType))
        {
            _cartridgePools.Add(ammoType,new 
            SimpleObjectPool<GameObject>((currentWeapon
            .WeaponComponent as WeaponComponentSimpleGun).FactoryMethod,null,40));
        }
        ShowCartridge(ammoType,_cartridgePools[ammoType].Allocate(),
        position, rotation,cartridgeLiveTime);
    }

    private void ShowCartridge(AmmoType ammoType,GameObject go,Vector3 position,Quaternion 
    rotation,float cartridgeLiveTime)
    {
        //Debug.Log("ShowCartridge");
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.SetActive(true);
        go.GetComponent<CartridgeLife>().SetCartridge(this,
            ammoType,cartridgeLiveTime);
    }

    public void RecycleCartridge(AmmoType ammoType,GameObject go)
    {
        _cartridgePools[ammoType].Recycle(go);
        go.SetActive(false);
    }
}
