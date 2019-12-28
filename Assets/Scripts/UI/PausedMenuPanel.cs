using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedMenuPanel : BasePanel
{
    private Button _resumeGameBtn;
    private Button _backToStartBtn;
    private Button _restartGameBtn;
    
    public override void Start()
    {
        base.Start();
        _resumeGameBtn = transform.Find("Menu/Continue")
            .GetComponent<Button>();
        _backToStartBtn = transform.Find("Menu/BackMenu")
            .GetComponent<Button>();
        _restartGameBtn = transform.Find("Menu/Restart")
            .GetComponent<Button>();
        _resumeGameBtn.onClick.AddListener(ResumeGame);
        _backToStartBtn.onClick.AddListener(BackToStart);
        _restartGameBtn.onClick.AddListener(RestartGame);
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
        SceneLoadingManager.Instance.LoadNewScene("StartScene");
    }

    private void RestartGame()
    {
        SceneLoadingManager.Instance.LoadNewScene("Demo_City");
    }

}
