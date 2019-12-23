using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponComponentRPG : WeaponComponent
{
    [HideInInspector]
    public Weapon owner;
    [HideInInspector]
    public GameObject projectilePrefab;
    private Transform _bulletSpawnpoint;
    private Light _flashLight;
    private ParticleSystem _smokePart;
    private ParticleSystem _sparkPart;

    private void Start()
    {
        _flashLight = GetComponentInChildren<Light>();
        _smokePart = transform.Find("Smoke Particles")
            .GetComponent<ParticleSystem>();
        _bulletSpawnpoint = transform.Find("Bullet Spawnpoint");
        _sparkPart = transform.Find("Spark Particles")
            .GetComponent<ParticleSystem>();
        _flashLight.enabled = false;
    }

    public override void Fire(Vector3 dir, GameObject cartridge = null)
    {
        _smokePart.Play();
        _sparkPart.Play();
        _flashLight.enabled = true;
        StartCoroutine(CeaseLight());
        Instantiate(projectilePrefab, _bulletSpawnpoint.position,
                _bulletSpawnpoint.rotation).GetComponent<Projectile>()
            .source = owner.gameObject;
        
    }

    IEnumerator CeaseLight()
    {
        yield return 1;
        _flashLight.enabled = false;
    }
}
