using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xcy.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponComponentSimpleGun : WeaponComponent
{

    public int cartridgeLiveTime;
    private Light _flashLight;
    private ParticleSystem _smokePart;
    private Transform _bulletSpawnpoint;
    private Transform _cartridgeSpawnpoint;

    public Transform CartridgeSpawnpoint => _cartridgeSpawnpoint;

    private SpriteRenderer[] _flashSprites=new SpriteRenderer[3];
    private ParticleSystem _bulletTracerPart;
    private ParticleSystem _sparkPart;

    private SimpleObjectPool<GameObject> _cartridgePool;
    private bool hasCreatePool = false;
    private GameObject _cartridgeGo;
    private void Awake()
    {
        _flashLight = GetComponentInChildren<Light>();
        _smokePart = transform.Find("Smoke Particles")
            .GetComponent<ParticleSystem>();
        _bulletSpawnpoint = transform.Find("Bullet Spawnpoint");
        _cartridgeSpawnpoint = transform.Find("Casing Spawnpoint");
        _flashSprites = GetComponentsInChildren<SpriteRenderer>();
        _bulletTracerPart = transform.Find("Bullet Tracer Particles")
            .GetComponent<ParticleSystem>();
        _sparkPart = transform.Find("Spark Particles")
            .GetComponent<ParticleSystem>();
        _bulletTracerPart.playbackSpeed = 2.5f;
        
        _flashLight.enabled = false;
        foreach (SpriteRenderer sprite in _flashSprites)
        {
            sprite.enabled = false;
        }
    }

    
    public override  void Fire(Vector3 dir,GameObject cartridge=null)
    {
        if (!hasCreatePool)
        {
            hasCreatePool = true;
            _cartridgeGo = cartridge;
//            _cartridgePool=new 
//                SimpleObjectPool<GameObject>(FactoryMethod,
//                    null,40);
        }
        //AllocateCartridge();
        PlayerWeaponController.Instance.AllocateCartridge
        (sourceWeapon.ammoType, _cartridgeSpawnpoint.position,
            _cartridgeSpawnpoint.rotation, cartridgeLiveTime);
        StartCoroutine(FlashSet(dir,cartridge));
    }

    IEnumerator FlashSet(Vector3 dir,GameObject cartridge)
    {
        _flashLight.enabled = true;
        foreach (SpriteRenderer sprite in _flashSprites)
        {
            sprite.enabled = true;
        }
        _smokePart.Play();
        _bulletTracerPart.transform.forward = dir;
        _bulletTracerPart.Play();
        _sparkPart.Play();
        yield return 1;
        _flashLight.enabled = false;
        foreach (SpriteRenderer sprite in _flashSprites)
        {
            sprite.enabled = false;
        }
    }
    
    public GameObject FactoryMethod()
    {
        GameObject tempGo= Instantiate(_cartridgeGo, 
            PlayerWeaponController.Instance.CurrentCartridgeSpawnpoint.position,
            PlayerWeaponController.Instance.CurrentCartridgeSpawnpoint.rotation,
            PlayerWeaponController.Instance.CartridgeHandle);
        tempGo.SetActive(false);
        return tempGo;
        
    }

//    private void AllocateCartridge()
//    {
//        ShowCartridge(_cartridgePool.Allocate());
//    }

//    public void ShowCartridge(GameObject go)
//    {
//        //Debug.Log("ShowCartridge");
//        go.transform.position = _cartridgeSpawnpoint.position;
//        go.transform.rotation = _cartridgeSpawnpoint.rotation;
//        go.GetComponent<CartridgeLife>().SetCartridge(this,cartridgeLiveTime);
//        go.SetActive(true);
//        //StartCoroutine(ResetCartridge(go));
//    }

//    public void RecycleCartridge(GameObject go)
//    {
//        go.SetActive(false);
//        _cartridgePool.Recycle(go);
//    }

    public override void CeaseFire()
    {
        if (_flashLight!=null)
        {
            _flashLight.enabled = false;
        }

        if (_flashSprites!=null)
        {
            foreach (SpriteRenderer sprite in _flashSprites)
            {
                sprite.enabled = false;
            }
        }
        
    }
}
