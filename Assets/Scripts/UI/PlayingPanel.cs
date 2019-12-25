using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingPanel : BasePanel
{
    [HideInInspector] public bool inThisPanel;
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&inThisPanel)
        {
            UIManager.Instance.PushPanel(UIType.PausedMenuUI);
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.Locked;
        inThisPanel = true;
    }

    public override void OnPaused()
    {
        inThisPanel = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnResume()
    {
        inThisPanel = true;
    }
    
}
