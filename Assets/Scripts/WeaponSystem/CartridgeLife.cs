using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CartridgeLife : MonoBehaviour
{
    private PlayerWeaponController _weaponController;
    
    public float minimumXForce;					
    public float maximumXForce;
    
    public float minimumYForce;
    public float maximumYForce;
    

    
    public AudioClip[] casingSounds;
    public AudioSource audioSource;

    private AmmoType _ammoType;
    private float _recycleTime;
    private Rigidbody _rigidbody;

    void Start () {
        
        //Debug.Log("Active");
        
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddRelativeForce (
            Random.Range (minimumXForce, maximumXForce), 
            Random.Range (minimumYForce, maximumYForce), 
            Random.Range (0, 0));
        StartCoroutine(Recycle());
    }

    void OnCollisionEnter (Collision collision) {
        
        audioSource.clip = casingSounds
            [Random.Range(0, casingSounds.Length)];
        audioSource.Play();
    }

    public void SetCartridge(PlayerWeaponController weaponController,
    AmmoType ammoType,float reTime)
    {
        _ammoType = ammoType;
        _recycleTime = reTime;
        this._weaponController = weaponController;
    }

    IEnumerator Recycle()
    {
        yield return new WaitForSeconds(_recycleTime);
        _weaponController.RecycleCartridge(_ammoType,this.gameObject);
    }

}
