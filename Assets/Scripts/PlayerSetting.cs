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

    public void SaveSettingData()
    {
        PlayerPrefs.SetFloat("MouseHorizontal",mouseHorizontalSensitivity);
        PlayerPrefs.SetFloat("MouseVertical",mouseVerticalSensitivity);
        PlayerPrefs.SetFloat("bgmVolume",bgmVolume);
        PlayerPrefs.SetFloat("soundEffectVolume",soundEffectVolume);
    }

}
