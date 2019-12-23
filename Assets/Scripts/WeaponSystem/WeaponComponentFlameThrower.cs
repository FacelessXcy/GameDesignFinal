using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xcy.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponComponentFlameThrower : WeaponComponent
{
    private Transform _overlapPoint;
    [HideInInspector]
    public MeleeWeaponCollider meleeWeaponCollider;
    private ParticleSystem _flamePart;
    private Light _flashLight;
    private bool _setTheDamage = false;
    private void Start()
    {
        _overlapPoint = transform.Find("OverlapPoint");
        _flashLight = GetComponentInChildren<Light>();
        _flamePart = transform.Find("Flame Particles")
            .GetComponent<ParticleSystem>();
        _flashLight.enabled = false;
        meleeWeaponCollider = _overlapPoint.GetComponent<MeleeWeaponCollider>();
    }


    public override void Fire(Vector3 dir, GameObject cartridge = null)
    {
        if (!_setTheDamage)
        {
            _setTheDamage = true;
            meleeWeaponCollider.sourceWeapon = sourceWeapon.gameObject;
            meleeWeaponCollider.damage = sourceWeapon.damage;
        }
        if (!_flamePart.isPlaying)
        {
            _flashLight.enabled = true;
            _flamePart.Play();
        }
        meleeWeaponCollider.Check(true);
    }


    public override void CeaseFire()
    {
        if (_flashLight==null)
        {
            _flashLight = GetComponentInChildren<Light>();
        }
        _flashLight.enabled = false;
        if (_flamePart==null)
        {
            _flamePart = transform.Find("Flame Particles")
                        .GetComponent<ParticleSystem>();
        }
        _flamePart.Stop();
        if (meleeWeaponCollider==null)
        {
            if (_overlapPoint==null)
            {
                _overlapPoint = transform.Find("OverlapPoint");
            }
            meleeWeaponCollider = _overlapPoint.GetComponent<MeleeWeaponCollider>();
        }
        meleeWeaponCollider.StopCheck();
    }
}
