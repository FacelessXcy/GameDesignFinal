using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuPanel : BasePanel
{

    private Button _startBtn;
    private Button _exitBtn;

    public override void Start()
    {
        base.Start();
        _startBtn = transform.Find("StartGameBtn")
            .GetComponent<Button>();
        _exitBtn= transform.Find("ExitGameBtn")
            .GetComponent<Button>();
        
        _startBtn.onClick.AddListener(() =>
        {
            SceneLoadingManager.Instance.LoadNewScene("Demo_City",null,"测试文字");
        });
        _exitBtn.onClick.AddListener(Application.Quit);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.None;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    public override void OnPaused()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }
    

}
