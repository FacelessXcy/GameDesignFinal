using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoSingleton<PlayerSetting>
{
    [HideInInspector] public float mouseHorizontalSensitivity;
    [HideInInspector] public float mouseVerticalSensitivity;
    [HideInInspector] public float bgmVolume;
    [HideInInspector] public float soundEffectVolume;
    
    protected override void OnDestroy()
    {
        SaveSettingData();
    }

    public override void Awake()
    {
        base.Awake();
        LoadSettingData();
    }

    public void LoadSettingData()
    {
        mouseHorizontalSensitivity= PlayerPrefs.GetFloat("MouseHorizontal",35f);
        mouseVerticalSensitivity=PlayerPrefs.GetFloat("MouseVertical",35f);
        bgmVolume=PlayerPrefs.GetFloat("bgmVolume",0.5f);
        soundEffectVolume=PlayerPrefs.GetFloat("soundEffectVolume",0.5f);
    }
    public void SaveSettingData()
    {
        PlayerPrefs.SetFloat("MouseHorizontal",mouseHorizontalSensitivity);
        PlayerPrefs.SetFloat("MouseVertical",mouseVerticalSensitivity);
        PlayerPrefs.SetFloat("bgmVolume",bgmVolume);
        PlayerPrefs.SetFloat("soundEffectVolume",soundEffectVolume);
    }

}
