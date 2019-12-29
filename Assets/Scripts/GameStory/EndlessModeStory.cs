using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeStory : GameStory
{
    
    public List<string> dialog;
    
    private int _dialogCount;
    private int _currentDialogIndex;
    private int _endDialogIndex;
    
    public float maxShowTime;
    private float _currentShowTime;
    private bool _isShowDialog;

    private bool _autoChangeToNextStateWhenFinishDialog;

    private int _currentRoundCount;
    private bool _isInRound;
    private int _needKillCount;
    private void Start()
    {
        _dialogCount = dialog.Count;
        UpdateStoryState(0);
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

        if (_isInRound)
        {
            if (EnemyManager.Instance.CurrentKillCount>=_needKillCount)
            {
                UpdateStoryState(2);
            }
        }
    }

    public override void UpdateStoryState(int stateIndex=-1)
    {
        if (stateIndex==-1)
        {
            stateIndex = _currentState;
        }

        stateIndex = stateIndex % 3;
        switch (stateIndex)
        {
            case 0:
                _currentRoundCount++;
                UIManager.Instance.SetMissionText("任务目标：活下去。");
                UpdateDialog(0,0,true);
                break;
            case 1:
                _isInRound = true;
                UpdateDialog(1,1,false);
                EnemyManager.Instance.MakeMonsterAtPosition(-1,5*_currentRoundCount,
                5*_currentRoundCount,false);
                _needKillCount = 10 * _currentRoundCount;
                break;
            case 2:
                _isInRound = false;
                UpdateDialog(2,2,true);
                break;
        }
        _currentState=stateIndex+1;
    }

    private void UpdateDialog(int beginIndex,int endIndex,bool autoChangeToNextState)
    {
        _currentDialogIndex = beginIndex;
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
            UIManager.Instance.SetDialogText
            ("第"+_currentRoundCount+dialog[_currentDialogIndex]);
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
