using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StartMenuPanel : BasePanel
{
    [FormerlySerializedAs("_endlessImage")] public Sprite endlessImage;
    private Button _startMissionBtn;
    private Button _startEndlessBtn;
    private Button _keyCodeBtn;
    private Button _exitBtn;
    private Button _settingBtn;
    public override void Start()
    {
        base.Start();
        _startMissionBtn = transform.Find("Mission").GetComponent<Button>();
        _startEndlessBtn = transform.Find("Endless").GetComponent<Button>();
        _keyCodeBtn = transform.Find("KeyCode").GetComponent<Button>();
        _exitBtn= transform.Find("Exit").GetComponent<Button>();
        _settingBtn = transform.Find("Setting").GetComponent<Button>();
        
        
        _startMissionBtn.onClick.AddListener(()=>
        {
            UIManager.Instance.PushPanel(UIType.DifficultUI);
        });
        
        _settingBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.PushPanel(UIType.SettingUI);
        });
        
        _startEndlessBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.difficult = DifficultMode.Hard;
            SceneLoadingManager.Instance.LoadNewScene("07 - wildwest",
            endlessImage,GameManager.Instance.endlessDescri
                );
        });
        _keyCodeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.PushPanel(UIType.KeyCodeUI);
        });
        _exitBtn.onClick.AddListener(Application.Quit);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }

    public override void OnPaused()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }

    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }
    

}
