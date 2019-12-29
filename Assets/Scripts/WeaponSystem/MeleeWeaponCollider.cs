using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class MeleeWeaponCollider : MonoBehaviour
{
    [HideInInspector] public GameObject sourceWeapon;
    [HideInInspector] public int damage;

    public AudioClip attackClip;
    public string[] attackLayer;
    private Collider _collider;
    private bool startCoroutine = false;
    private AudioSource _audioSource;
    private void Start()
    {
        _collider=GetComponent<Collider>();
        _collider.isTrigger = true;
        _collider.enabled = false;
        _audioSource = GetComponent<AudioSource>();
        if (attackClip!=null)
        {
            if (_audioSource==null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.outputAudioMixerGroup =
                    AudioManager.Instance.audioMixer
                        .FindMatchingGroups("SoundEffect")[0];
                _audioSource.playOnAwake = false;
            }
            //_audioSource.clip = attackClip;
        }
    }

    public void Check(bool autoClose)
    {
        if (autoClose)
        {
            if (!startCoroutine)
            {
                _collider.enabled = true;
                startCoroutine = true;
                StartCoroutine(CloseCollider());
            }
        }
        else
        {
            _collider.enabled = true;
        }

    }

    IEnumerator CloseCollider()
    {
        yield return new WaitForSeconds(0.4f);
        startCoroutine = false;
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Damageable>()!=null)
        {
            foreach (string s in attackLayer)
            {
                if (other.gameObject.layer==LayerMask.NameToLayer(s))
                {
                    if (attackClip!=null)
                    {
                        //播放音效
                        _audioSource.PlayOneShot(attackClip);
                    }
                    other.GetComponent<Damageable>().GetDamage(damage,sourceWeapon);
                }
            }
            
        }
    }
    
    public void StopCheck()
    {
        StopCoroutine(CloseCollider());
        startCoroutine = false;
        if (_collider==null)
        {
            _collider=GetComponent<Collider>();
            _collider.isTrigger = true;
        }
        _collider.enabled = false;
    }
}
