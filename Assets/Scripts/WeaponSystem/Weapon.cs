using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum FireType
{
    SemiAutomatic,
    Automatic
}

public enum AmmoType
{
    NoneAmmo,
    PistolAmmo,
    AssaultRifleAmmo,
    Oil,
    Rocket
}

public enum WeaponType
{
    Melee,
    Pistol,
    SimpleWeapon,
    SpecialWeapon
}


public class Weapon : MonoBehaviour
{

    public WeaponData weaponData;
    [HideInInspector] public Vector3 offset;
    [HideInInspector] public bool isFlameThrower;
    [HideInInspector] public bool isRPG;
    [HideInInspector] public bool isMeleeWeapon;
    [HideInInspector] public string weaponName;
    [HideInInspector] public AmmoType ammoType;
    [HideInInspector] public FireType fireType;
    [HideInInspector] public WeaponType weaponType;
    [HideInInspector] public int damage;
    [HideInInspector] public float range;
    [HideInInspector] public int maxAmmo;
    [HideInInspector] public GameObject cartridge;
    [HideInInspector] public GameObject bulletHole;
    [HideInInspector] public float bulletSpreadAngle;
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public GameObject dropWeaponPrefab;
    [HideInInspector] public Sprite crosshair;
    private float _fireTime;
    private float _reloadTime;
    private AudioClip _fireClip;
    private AudioClip _reloadClip;
    private AudioClip _dryFireClip;

    private WeaponComponent _weaponComponent;
    public WeaponComponent WeaponComponent => _weaponComponent;

    [HideInInspector]
    public PlayerWeaponController owner;

    private Animator _animator;
    private Camera _fpsCamera;
    private Ray _ray;
    private RaycastHit _hit;
    private GameObject _hitGo;
    private AudioSource _audioSource;
    private int _curAmmo;

    public int CurAmmo
    {
        get => _curAmmo;
        set
        {
            _curAmmo = value;
            if (_curAmmo>maxAmmo)
            {
                _curAmmo = maxAmmo;
            }
        }
    }

    private float _nextFireTime = 0.0f;
    private int _reloadAmount = 0;//本次换弹消耗数量
    private float _finishReloadTime;
    

    private float _startReloadTime;
    private float _endReloadTime;
    private float _startFireTime;
    private float _endFireTime;
    //热兵器动作
    private int _shootID = Animator.StringToHash("Shoot");
    private int _reloadID=Animator.StringToHash("Reload");
    private int _runID=Animator.StringToHash("Run");
    private int _drawID=Animator.StringToHash("Draw");
    private int _jumpID=Animator.StringToHash("Jump");
    //冷兵器动作
    
    private bool _isReloading;
    private bool _isShooting;
    private bool _isFlaming=false;
    public bool IsShooting => _isShooting;
    private bool _isDrawing;
    private bool _isRunning;
    private bool _isJumping;
    
    private bool _lastReloading;

    private bool _autoShoot = false;
    private int _autoShootCount = 0;
    private float _recoilResetTime = 0.5f;
    private float _recoilResetInTime = 0;
    
    //temp value
    private float _spreadAngleRatio;
    private Vector3 _spreadWorldDirection;
    private Vector3 _tempShootDirection;
    private Collider[] _overlapCols;
    private void Awake()
    {
        InitData();
        _curAmmo = maxAmmo;
    }
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _fpsCamera=Camera.main;
        transform.localPosition = offset;
        if (_weaponComponent==null)
        {
            if (isFlameThrower)
            {
                _weaponComponent = 
                    GetComponentInChildren<WeaponComponentFlameThrower>();
            }
            else if (isRPG)
            {
                _weaponComponent =
                    GetComponentInChildren<WeaponComponentRPG>();
            }
            else if (isMeleeWeapon)
            {
                _weaponComponent =
                    GetComponentInChildren<WeaponComponentMelee>();
            }
            else
            {
                _weaponComponent=GetComponentInChildren<WeaponComponentSimpleGun>();
            }
        }

        _weaponComponent.sourceWeapon = this;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource==null)
        {
            gameObject.AddComponent<AudioSource>().playOnAwake = false;
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
        _audioSource.clip = _reloadClip;
        _nextFireTime = 0.0f;
        _ray=new Ray();
    }

    private void Update()
    {
        FlameAudioControl();

        if (_autoShoot&&Time.time>=_recoilResetInTime)
        {
            _autoShoot = false;
            //Debug.Log("_autoShoot = false");
            _autoShootCount = 0;
        }
        Jump();
        Run();
        AnimationCheck();
        if (!_isReloading)
        {
            if (_lastReloading)
            {
                owner.RemoveAmmoAmount(ammoType, _reloadAmount-_curAmmo);
                _curAmmo = _reloadAmount;
                Debug.Log("CurrentAmmo:"+_curAmmo); 
            }
        }

        if (isMeleeWeapon&&!_isShooting)
        {
            ShootMeleeCeaseFire();
        }
        _lastReloading=_isReloading;
    }

    private void OnDisable()
    {
        if (isFlameThrower)
        {
            FlameCeaseFire();
        }
        else if (isMeleeWeapon)
        {
            ShootMeleeCeaseFire();
        }
        else
        {
            SimpleGunCeaseFire();
        }
        _isReloading = false;
        _lastReloading = false;
    }

    public bool ShootInput(bool inputDown,bool inputHold,bool inputUp)
    {
        switch (fireType)
        {
            case FireType.SemiAutomatic:
                if (inputDown)
                {
                    return TryShoot();
                }
                return false;
            case FireType.Automatic:
                if (inputHold)
                {
                    return TryShoot();
                }
                if (isFlameThrower)
                {
                    FlameCeaseFire();
                }
                return false;
            default:
                if (isFlameThrower)
                {
                    FlameCeaseFire();
                }
                return false;
        }
    }

    private bool TryShoot()
    {
        if (isFlameThrower)
        {
            if (!_isRunning&&!_isDrawing&&!_isReloading
                &&_nextFireTime<=Time.time)
            {
                _nextFireTime = Time.time + _fireTime;
                if (_curAmmo>=1)
                {
                    Shoot();
                    return true;
                }
                FlameCeaseFire();
            }
        }
        else if (isMeleeWeapon)
        {
            if (!_isDrawing && !_isShooting &&
                _nextFireTime <= Time.time)
            {
                _nextFireTime = Time.time + _fireTime;
                Shoot();
            }
        }
        else
        {
            if (!_isRunning&&!_isDrawing&&!_isShooting&&!_isReloading
                &&_nextFireTime<=Time.time)
            {
                _nextFireTime = Time.time + _fireTime;
                if (_curAmmo>=1)
                {
                    Shoot();
                    return true;
                } 
                if (isRPG)
                {
                    //Do Nothing
                }
                else
                {
                    owner.bulletAudio.PlayOneShot(_dryFireClip);
                }
            }
        }
        
        return false;
    }

    private void Shoot()
    {
        if (isFlameThrower)
        {
            _isFlaming = true;
        }
        else
        {
            if (owner.flameThrowerAudio.isPlaying)
            {
                owner.flameThrowerAudio.Stop();
            }
            owner.bulletAudio.PlayOneShot(_fireClip);
        }

        if (!isMeleeWeapon)
        {
            _curAmmo -= 1;
            if (!_autoShoot)
            {
                _autoShoot = true;
            }
            _autoShootCount += 1;
            _recoilResetInTime = Time.time + _recoilResetTime;
        }
        
        _isShooting = true;
        _animator.SetTrigger(_shootID);
        //Debug.Log("CurrentAmmo:"+_curAmmo);
        if (isFlameThrower)
        {
            ShootFlame();
        }
        else if (isRPG)
        {
            ShootRPG();
        }else if (isMeleeWeapon)
        {
            ShootMeleeFire();
        }
        else
        {
            ShootRayCast();
        }

        
    }

    public bool ReloadInput(bool inputDown)
    {
        if (!isMeleeWeapon)
        {
            if (inputDown)
            {
                return TryReloadAmmo();
            }
        }
        return false;
    }

    private bool TryReloadAmmo()
    {
        if (_isReloading||_isShooting||_isDrawing||(_curAmmo==maxAmmo))
        {
            return false;
        }
        int currentAmmoAmount = owner.GetAmmoAmount(ammoType);
        if (currentAmmoAmount<=0)
        {
            Debug.Log("无备用子弹： "+ammoType);
            return false;
        }
        int removeAmount = maxAmmo-_curAmmo;
        if (currentAmmoAmount<removeAmount)
        {
            removeAmount = currentAmmoAmount;
        }
        ReloadAmmo(removeAmount);
        return true;
    }
    

    private void ReloadAmmo(int reloadAmount)
    {
        _finishReloadTime = Time.time + _reloadTime;
        _isReloading = true;
        _audioSource.PlayOneShot(_reloadClip);
        _animator.SetTrigger(_reloadID);
        _reloadAmount = _curAmmo+reloadAmount;
    }

    private void Jump()
    {
        if (PlayerMover.Instance.isJump)
        {
            _animator.SetTrigger(_jumpID);
        }
    }

    private void Run()
    {
        if (!_isShooting&&PlayerMover.Instance.isRun&&
            !PlayerMover.Instance.isJump)
        {
            _animator.SetFloat(_runID,0.2f);
        }
        else
        {
            _animator.SetFloat(_runID,0.0f);
        }
    }

    private void AnimationCheck()
    {
        //Check if shooting
		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("Fire")) {
			_isShooting = true;
            if (_startFireTime==0)
            {
                _startFireTime = Time.time;
            }
		} else {
            if (Time.time>=_nextFireTime)
            {
                _isShooting = false;
            }
            //测试数据用
            if (_startFireTime!=0&&_endFireTime==0)
            {
                _endFireTime = Time.time;
                //Debug.Log(weaponName+"_fireTime:"+(_endFireTime - _startFireTime));
            }
		}

        //Check if running
		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
            _isRunning = true;
		} else {
            _isRunning = false;
		}
		
		//Check if jumping
		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
            _isJumping = true;
		} else {
            _isJumping = false;
		}

		//Check if drawing weapon
		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("Draw")) {
            _isDrawing = true;
		} else {
            _isDrawing = false;
		}

        //Check if reloading
		if (_animator.GetCurrentAnimatorStateInfo (0).IsName ("Reload")) {
			// If reloading
            _isReloading = true;
            if (_startReloadTime==0)
            {
                _startReloadTime = Time.time;
            }
        }
        else
        {
            if (Time.time>=_finishReloadTime)
            {
                _isReloading = false;
            }
            //测试数据用
            if (_startReloadTime!=0&&_endReloadTime==0)
            {
                _endReloadTime = Time.time;
                //Debug.Log(weaponName+"_reloadTime:"+(_endReloadTime - 
                //_startReloadTime));
            }
        }

    }

    public void DrawWeapon()
    {
        if (isFlameThrower)
        {
            if (!owner.flameThrowerAudio.isPlaying)
            {
                owner.flameThrowerAudio.Stop();
            }
            owner.flameThrowerAudio.clip = _fireClip;
            owner.bulletAudio.loop = true;
        }
        owner.bulletAudio.volume = 1;
        _isDrawing = true;
        if (_animator==null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.SetTrigger(_drawID);
    }

    private void InitData()
    {
        crosshair = weaponData.crosshair;
        offset = weaponData.offset;
        isFlameThrower = weaponData.isFlameThrower;
        isRPG = weaponData.isRPG;
        isMeleeWeapon = weaponData.isMeleeWeapon;
        bulletHole = weaponData.bulletHole;
        weaponName = weaponData.weaponName;
        fireType = weaponData.fireType;
        ammoType = weaponData.ammoType;
        weaponType = weaponData.weaponType;
        if (!isFlameThrower&&!isRPG&&!isMeleeWeapon)
        {
            cartridge = weaponData.cartridge;
        }
        range = weaponData.range;
        damage = weaponData.damage;
        _reloadTime = weaponData.reloadTime;
        _fireTime = weaponData.fireTime;
        maxAmmo = weaponData.maxAmmo;
        bulletSpreadAngle = weaponData.bulletSpreadAngle;
        _fireClip = weaponData.fireClip;
        _dryFireClip = weaponData.dryFireClip;
        _reloadClip = weaponData.reloadClip;
        dropWeaponPrefab = weaponData.dropWeaponPrefab;
        if (isRPG)
        {
            projectilePrefab = weaponData.projectilePrefab;
        }
    }

    private void ShootRayCast()
    {
        _tempShootDirection = GetShootDirectionWithinSpread(_fpsCamera.transform);
        _ray.origin = _fpsCamera.transform.position;
        _ray.direction = _tempShootDirection;
        //枪口火光,以及弹道拖尾
        Debug.DrawRay(_fpsCamera.transform.position,
        _tempShootDirection*range,Color.black,2);
        _weaponComponent.Fire(_tempShootDirection,cartridge);
        if (Physics.Raycast(_ray,out _hit,range))
        {
            //Debug.Log(_hit.collider.name+"被击中");
            if (_hit.collider.CompareTag("Player"))
            {
                return;
            }
            Damageable damageable =
                _hit.collider.GetComponentInParent<Damageable>();
            if (damageable!=null)
            {
                damageable.GetDamage(damage,this.gameObject);
                return;
            }
        
            if (bulletHole!=null)
            {
                if (_hit.collider.isTrigger!=true)
                {
                    Instantiate(bulletHole, _hit.point, 
                        Quaternion.FromToRotation(Vector3.forward,_hit
                            .normal)).transform.parent=_hit.collider.transform;
                }
            }
        }
    }

    private void ShootFlame()
    {
        (_weaponComponent as WeaponComponentFlameThrower).sourceWeapon = this;
        _weaponComponent.Fire(Vector3.zero,null); 
    }

    private void ShootRPG()
    {
        if ((_weaponComponent as WeaponComponentRPG).owner!=this)
        {
            (_weaponComponent as WeaponComponentRPG).owner = this;
        }
        (_weaponComponent as WeaponComponentRPG).projectilePrefab=projectilePrefab;
        _weaponComponent.Fire(Vector3.zero);
    }

    private void ShootMeleeFire()
    {
        (_weaponComponent as WeaponComponentMelee).sourceWeapon = this;
        _weaponComponent.Fire(Vector3.zero);
    }

    private void ShootMeleeCeaseFire()
    {
        _weaponComponent.CeaseFire();
        
    }

    public Vector3 GetShootDirectionWithinSpread(Transform shootTransform)
    {
        _spreadAngleRatio = bulletSpreadAngle / 180f;
        if (_autoShootCount<=4)
        {
            _spreadAngleRatio=_spreadAngleRatio * _autoShootCount / 8;
        }
        
        _spreadWorldDirection = Vector3.Slerp(shootTransform.forward, 
            UnityEngine.Random.insideUnitSphere,
            _spreadAngleRatio);
        return _spreadWorldDirection;
    }

    private void FlameCeaseFire()
    {
        _isFlaming = false;
        if (_weaponComponent==null)
        {
            _weaponComponent=GetComponentInChildren<WeaponComponentFlameThrower>();
        }
        _weaponComponent.CeaseFire();
    }

    private void SimpleGunCeaseFire()
    {
        if (_weaponComponent==null)
        {
            if (isRPG)
            {
                _weaponComponent=GetComponentInChildren<WeaponComponentRPG>();
            }
            else
            {
                _weaponComponent=GetComponentInChildren<WeaponComponentSimpleGun>();
            }
        }
        _weaponComponent.CeaseFire();
    }

    private void FlameAudioControl()
    {
        if (_isFlaming)
        {
            if (owner.flameThrowerAudio.volume!=1.0f)
            {
                owner.flameThrowerAudio.volume = 1.0f;
            }
            if (!owner.flameThrowerAudio.isPlaying)
            {
                owner.flameThrowerAudio.Play();
            }
        }
        else
        {
            if (owner.flameThrowerAudio.volume>0.0f)
            {
                owner.flameThrowerAudio.volume -= 0.05f;
            }
            else
            {
                owner.flameThrowerAudio.Stop();
            }
        }
    }

}
