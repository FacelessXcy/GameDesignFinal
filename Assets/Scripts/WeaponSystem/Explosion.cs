using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Explosion : MonoBehaviour
{

    private AudioSource _audioSource;
    public AudioClip explosionClip;
    public bool isMonsterExplosion;
    private ParticleSystem[] _particleSystems;

    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = explosionClip;
    }

    private void OnEnable()
    {
        //Debug.Log("OnEnable_particleSystems==null"+(_particleSystems==null));
        _audioSource.Play();
        foreach (ParticleSystem system in _particleSystems)
        {
            system.Play();
        }
        StartCoroutine(Hide());
    }

    public void OnDisable()
    {
        foreach (ParticleSystem system in _particleSystems)
        {
            if (system==null)
            {
                //Debug.Log("system==null   "+(system==null));
            }
            system.Stop();
        }
    }


    IEnumerator Hide()
    {
        yield return new  WaitForSeconds(3.0f);
        if (isMonsterExplosion)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log(" Destroy(gameObject)");
            Destroy(gameObject);
        }
    }
    


}
