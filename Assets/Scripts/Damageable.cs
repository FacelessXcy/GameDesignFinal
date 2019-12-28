using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Damageable : MonoBehaviour
{
    private Health _health;
    

    private void Start()
    {
        _health = GetComponent<Health>();
    }


    public void GetDamage(int damage,GameObject damageResource)
    {
        //Debug.Log(name+"被"+damageResource.name+"击中");
        if (_health!=null)
        {
            _health.TakeDamage(damage,damageResource);
        }
    }

}
