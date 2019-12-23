using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponComponentMelee : WeaponComponent
{

    private MeleeWeaponCollider _weaponCollider;
    private bool _setDamage = false;
    private void Start()
    {
        _weaponCollider = GetComponent<MeleeWeaponCollider>();

    }

    public override void Fire(Vector3 dir, GameObject cartridge = null)
    {
        if (!_setDamage)
        {
            _setDamage = true;
            _weaponCollider.sourceWeapon = sourceWeapon.gameObject;
            _weaponCollider.damage = sourceWeapon.damage;
        }

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.4f);
        _weaponCollider.Check(false);
    }

    public override void CeaseFire()
    {
        if (_weaponCollider==null)
        {
            _weaponCollider = GetComponent<MeleeWeaponCollider>();
        }
        _weaponCollider.StopCheck();
    }
}
