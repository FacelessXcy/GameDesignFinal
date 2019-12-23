using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponComponent : MonoBehaviour
{
    [HideInInspector]
    public Weapon sourceWeapon;

    
    
    public virtual void Fire(Vector3 dir,GameObject cartridge=null)
    {
        
    }    

    public virtual void CeaseFire()
    {
        
    }

}
