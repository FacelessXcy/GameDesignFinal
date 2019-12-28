using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuPanel : BasePanel
{
    public Sprite _missionImage;
    public Sprite _endlessImage;
    private Button _startMissionBtn;
    private Button _startEndlessBtn;
    private Button _bestRecord;
    private Button _exitBtn;

    public override void Start()
    {
        base.Start();
        _startMissionBtn = transform.Find("Mission")
            .GetComponent<Button>();
        _startEndlessBtn = transform.Find("Endless")
            .GetComponent<Button>();
        _bestRecord = transform.Find("Record")
            .GetComponent<Button>();
        _exitBtn= transform.Find("Exit")
            .GetComponent<Button>();
        
        _startMissionBtn.onClick.AddListener(() =>
        {
            SceneLoadingManager.Instance.LoadNewScene("Demo_City",
            _missionImage,GameManager.Instance.missionDescri);
        });
        _startEndlessBtn.onClick.AddListener(() =>
        {
            SceneLoadingManager.Instance.LoadNewScene("SampleScene",
            _endlessImage,GameManager.Instance.endlessDescri
                );
        });
        
        _exitBtn.onClick.AddListener(Application.Quit);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
