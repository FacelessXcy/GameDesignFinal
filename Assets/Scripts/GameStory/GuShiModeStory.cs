using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuShiModeStory : GameStory
{
    public List<string> dialog;
    
    private int _dialogCount;
    private int _currentDialogIndex;
    private int _endDialogIndex;
    
    public float maxShowTime;
    private float _currentShowTime;
    private bool _isShowDialog;

    private bool _autoChangeToNextStateWhenFinishDialog;

    private void Start()
    {
        _dialogCount = dialog.Count;
        UpdateStoryState();
    }

    private void Update()
    {
        if (_isShowDialog)
        {
            _currentShowTime += Time.deltaTime;
            if (_currentShowTime>=maxShowTime)
            {
                _currentShowTime = 0;
                UpdateDialogHandle();
            }
        }
    }

    public override void UpdateStoryState(int stateIndex=-1)
    {
        if (stateIndex==-1)
        {
            stateIndex = _currentState;
        }
        switch (stateIndex)
        {
            case 0:
                UpdateDialog(4,true);
                break;
            case 1:
                //任务提示：沿着公路,前往城南的大桥，进入隧道...
                UIManager.Instance.SetMissionText("沿着公路,穿过城市,前往城南的大桥，进入隧道");
                break;
            case 2:
                //自言自语：什么声音？
                UpdateDialog(5,false);
                EnemyManager.Instance.MakeMonsterAtPosition(0,20,30,false);
                break;
            case 3:
                UpdateDialog(6,false);
                break;
            case 4:
                UpdateDialog(7,false);
                EnemyManager.Instance.MakeMonsterAtPosition(1,15,30);
                break;
            case 5:
                UpdateDialog(8,false);
                EnemyManager.Instance.MakeMonsterAtPosition(2,15,25);
                break;
            case 6:
                UpdateDialog(9,false);
                EnemyManager.Instance.MakeMonsterAtPosition(3,10,10,false);
                break;
            case 7:
                UIManager.Instance.SetResultText("任务完成");
                GameManager.Instance.PauseGame();
                UIManager.Instance.PushPanel(UIType.EndMenuUI);
                break;
            default:
                break;
        }
        _currentState=stateIndex+1;
    }

    private void UpdateDialog(int endIndex,bool autoChangeToNextState)
    {
        _endDialogIndex = endIndex;
        _autoChangeToNextStateWhenFinishDialog = autoChangeToNextState;
        UpdateDialogHandle();
    }

    private void UpdateDialogHandle()
    {
        if (_currentDialogIndex<=_endDialogIndex)
        {
            //更新显示
            _isShowDialog = true;
            UIManager.Instance.SetDialogText(dialog[_currentDialogIndex]);
            _currentDialogIndex++;
        }
        else
        {
            UIManager.Instance.SetDialogText("");
            _isShowDialog = false;
            if (_autoChangeToNextStateWhenFinishDialog)
            {
                UpdateStoryState();
            }
        }

    }


}
