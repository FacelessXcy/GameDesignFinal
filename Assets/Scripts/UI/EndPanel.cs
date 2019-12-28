using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : BasePanel
{
    private Button _restartGameBtn;
    private Button _backToStartBtn;
    private Text _resultText;
    public override void Start()
    {
        base.Start();
        _restartGameBtn = transform.Find("Menu/Restart")
            .GetComponent<Button>();
        _backToStartBtn = transform.Find("Menu/BackMenu")
            .GetComponent<Button>();
        _resultText = transform.Find("Menu/ResultText")
        .GetComponent<Text>();
        _restartGameBtn.onClick.AddListener(() =>
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SceneLoadingManager.Instance.LoadNewScene(sceneName,
                null,
                sceneName=="Demo_City"?
                    GameManager.Instance.missionDescri:
                    GameManager.Instance.endlessDescri);
        });
        _backToStartBtn.onClick.AddListener(() =>
        {
            SceneLoadingManager.Instance.LoadNewScene("StartScene");
        });
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    public override void OnExit()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    public void SetResultText(string str)
    {
        if (_resultText==null)
        {
            _resultText = transform.Find("Menu/ResultText")
                    .GetComponent<Text>();
        }

        _resultText.text = str;
    }

}
