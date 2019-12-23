using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RealBuilding : MonoBehaviour
{
    public AudioClip hitClip;
    
    private Health _health;
    private AudioSource _audioSource;
    private void Start()
    {
        _health = GetComponent<Health>();
        _audioSource=GetComponent<AudioSource>();
        _health.onDamaged += OnDamaged;
        _health.onDied += OnDied;
    }

    private void OnDamaged(float damage,GameObject damageResource)
    {
        
        _audioSource.PlayOneShot(hitClip);
        
    }

    private void OnDied()
    {
        _audioSource.PlayOneShot(hitClip);
        Destroy(this.gameObject);
    }

}
