using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyCodePanel : BasePanel
{
    private Button _backBtn;
    public override void Start()
    {
        base.Start();
        _backBtn = GetComponent<Button>();
        _backBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.PopPanel();
        });
    }

    public override void OnEnter()
    {
        base.OnEnter();
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
