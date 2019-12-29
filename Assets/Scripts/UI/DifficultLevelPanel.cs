using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultLevelPanel : BasePanel
{
    public Sprite loadingImage;
    private Button _simpleBtn;
    private Button _hardBtn;
    private Button _backBtn;
    public override void Start()
    {
        base.Start();
        _simpleBtn =
            transform.Find("Menu/Simple").GetComponent<Button>();
        _hardBtn = transform.Find("Menu/Difficult")
            .GetComponent<Button>();
        _backBtn = GetComponent<Button>();
        _simpleBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.difficult = DifficultMode.Easy;
            SceneLoadingManager.Instance.LoadNewScene("Demo_City",
                loadingImage,GameManager.Instance.missionDescri);
        });
        _hardBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.difficult = DifficultMode.Hard;
            SceneLoadingManager.Instance.LoadNewScene("Demo_City",
                loadingImage,GameManager.Instance.missionDescri);
        });
        _backBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.PopPanel();
        });
        
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

    public override void OnExit()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }
}
