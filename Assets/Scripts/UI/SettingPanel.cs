using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private Slider _verticalSlider;
    private Slider _horizontalSlider;
    private Slider _bgmVolumeSlider;
    private Slider _soundEffectSlider;
    private Button _backBtn;
    public override void Start()
    {
        base.Start();
        _verticalSlider = transform.Find("Menu/MouseSensorVertical")
            .GetComponent<Slider>();
        _horizontalSlider = transform.Find("Menu/MouseSensorHorizontal")
            .GetComponent<Slider>();
        
        _bgmVolumeSlider = transform.Find("Menu/BGM_Volume")
            .GetComponent<Slider>();
        _soundEffectSlider = transform.Find("Menu/EffectVolume")
            .GetComponent<Slider>();
        _backBtn = GetComponent<Button>();
        _backBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.PopPanel();
        });
        
        _verticalSlider.onValueChanged.AddListener((value) =>
            {
                PlayerSetting.Instance.mouseVerticalSensitivity = value;
            });
        _horizontalSlider.onValueChanged.AddListener((value) =>
        {
            PlayerSetting.Instance.mouseHorizontalSensitivity = value;
        });
        _bgmVolumeSlider.onValueChanged.AddListener((value) =>
        {
            PlayerSetting.Instance.bgmVolume = value;
            AudioManager.Instance.SetBGMVolume();
        });
        _soundEffectSlider.onValueChanged.AddListener((value) =>
        {
            PlayerSetting.Instance.soundEffectVolume = value;
            AudioManager.Instance.SetSoundEffectVolume();
        });

        _soundEffectSlider.value =
            PlayerPrefs.GetFloat("soundEffectVolume", 5f);
        _bgmVolumeSlider.value =
            PlayerPrefs.GetFloat("bgmVolume", -20f);
        _verticalSlider.value =
            PlayerPrefs.GetFloat("MouseVertical", 35f);
        _horizontalSlider.value =
            PlayerPrefs.GetFloat("MouseHorizontal", 35f);

    }

    public override void OnEnter()
    {
        base.OnEnter();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    public override void OnExit()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        
        PlayerSetting.Instance
            .mouseHorizontalSensitivity = _horizontalSlider.value;
        
        PlayerSetting.Instance
            .mouseVerticalSensitivity = _verticalSlider.value;
        
        PlayerSetting.Instance
            .bgmVolume = _bgmVolumeSlider.value;
        
        PlayerSetting.Instance
            .soundEffectVolume = _soundEffectSlider.value;
        PlayerSetting.Instance.SaveSettingData();
        if (SceneManager.GetActiveScene().name!="StartScene")
        {
            PlayerMover.Instance.SetPlayerSetting();
        }
    }
}
