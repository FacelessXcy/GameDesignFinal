using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioMixer audioMixer;
    private AudioSource _musicAudioSource;
    public AudioClip[] musicList;
    private int _currentPlayIndex=0;
    private void Start()
    {
        _musicAudioSource = GetComponent<AudioSource>();
        if (_musicAudioSource==null)
        {
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
        }
        _musicAudioSource.loop = false;
        _musicAudioSource.playOnAwake = false;
        SetBGMVolume();
        SetSoundEffectVolume();
    }

    private void Update()
    {
        if (!_musicAudioSource.isPlaying)
        {
            UpdateAudioClip();
        }
    }

    private void UpdateAudioClip()
    {
        _musicAudioSource.clip = musicList[_currentPlayIndex%
        (musicList.Length)];
        _musicAudioSource.Play();
        _currentPlayIndex++;
    }

    public void SetBGMVolume()
    {
        audioMixer.SetFloat("MasterVolume",
            PlayerSetting.Instance.bgmVolume);
    }

    public void SetSoundEffectVolume()
    {
        audioMixer.SetFloat("SoundEffectVolume",
            PlayerSetting.Instance.soundEffectVolume);
    }

}
