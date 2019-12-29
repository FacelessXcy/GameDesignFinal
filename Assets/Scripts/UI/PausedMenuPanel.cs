using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausedMenuPanel : BasePanel
{
    private Button _resumeGameBtn;
    private Button _backToStartBtn;
    private Button _restartGameBtn;
    private Button _settingBtn;
    
    public override void Start()
    {
        base.Start();
        _resumeGameBtn = transform.Find("Menu/Continue")
            .GetComponent<Button>();
        _backToStartBtn = transform.Find("Menu/BackMenu")
            .GetComponent<Button>();
        _restartGameBtn = transform.Find("Menu/Restart")
            .GetComponent<Button>();
        _settingBtn = transform.Find("Menu/Setting")
            .GetComponent<Button>();
        _resumeGameBtn.onClick.AddListener(ResumeGame);
        _backToStartBtn.onClick.AddListener(BackToStart);
        _restartGameBtn.onClick.AddListener(RestartGame);
        _settingBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.PushPanel(UIType.SettingUI);
        });
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts=true;
        canvasGroup.interactable = true;
        GameManager.Instance.PauseGame();
    }

    public override void OnPaused()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts=false;
        canvasGroup.interactable = false;
    }

    public override void OnResume()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts=true;
        canvasGroup.interactable = true;
    }

    public override void OnExit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts=false;
        canvasGroup.interactable = false;
    }

    private void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        UIManager.Instance.PopPanel();
    }

    private void BackToStart()
    {
        SceneLoadingManager.Instance.LoadNewScene("StartScene",null,GameManager.Instance.randomDescri[Random.Range(0,
            GameManager.Instance.randomDescri.Length)]);
    }

    private void RestartGame()
    {
        SceneLoadingManager.Instance.LoadNewScene(SceneManager
        .GetActiveScene().name,null,GameManager.Instance.randomDescri[Random.Range(0,
            GameManager.Instance.randomDescri.Length)]);
    }

}
