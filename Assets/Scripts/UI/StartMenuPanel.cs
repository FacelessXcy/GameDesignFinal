using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuPanel : BasePanel
{

    private Button _startBtn;
    private Button _exitBtn;

    private void Start()
    {
        _startBtn = transform.Find("StartGameBtn")
            .GetComponent<Button>();
        _exitBtn= transform.Find("ExitGameBtn")
            .GetComponent<Button>();
        
        _startBtn.onClick.AddListener(() =>
        {
            OnPushPanel(UIType.PlayingUI);
        });
        Debug.Log("_startBtn.onClick.AddListener");
        
    }


    public void OnPushPanel(UIType uiType)
    {
        Debug.Log("Click");
        UIManager.Instance.PushPanel(uiType);
    }

}
