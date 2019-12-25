using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyFSMSystem 
{
    private List<EnemyFSMState> _states;
    private EnemyFSMState _currentState;
    public EnemyFSMState CurrentState => _currentState;
    public EnemyControllerBase character;
    
    public bool enabled = true;
    public EnemyFSMSystem(EnemyControllerBase character)
    {
        this.character = character;
        _states=new List<EnemyFSMState>();
    }


    public void Update(Transform player)
    {
        if (!enabled)
        {
            return;
        }
        _currentState.Reason(player);
        _currentState.Act(player);
    }

    public void AddStates(params EnemyFSMState[] states)
    {
        foreach (EnemyFSMState state in states)
        {
            AddState(state);
        }
    }

    public void AddState(EnemyFSMState state)
    {
        if (_states == null)
        {
            _states = new List<EnemyFSMState>();
        }

        if (state == null)
        {
            Debug.LogError("要添加的状态为空");
            return;
        }

        if (_states.Count == 0)
        {
            _states.Add(state);
            _currentState = state;
            _currentState.DoBeforeEntering();
            return;
        }
        foreach (EnemyFSMState fsmState in _states)
        {
            if (fsmState.StateID == state.StateID)
            {
                Debug.Log("要添加的状态ID" + state.StateID + "已添加");
                return;
            }
        }
        _states.Add(state);
    }

    public void DeleteState(StateID stateID)
    {
        if (stateID == StateID.NullState)
        {
            Debug.LogError("要删除的状态ID[" + stateID + "]为空状态");
            return;
        }

        foreach (EnemyFSMState state in _states)
        {
            if (state.StateID == stateID)
            {
                _states.Remove(state);
                return;
            }
        }

        Debug.LogError("要删除的状态ID[" + stateID + "]不存在");
    }

    /// <summary>
    /// 执行转换
    /// </summary>
    /// <param name="trans">转换条件</param>
    public void PerformTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("敌人状态 要执行的转换条件为空" + trans);
            return;
        }

        StateID nextStateID = _currentState.GetOutputState(trans);
        if (nextStateID == StateID.NullState)
        {
            Debug.LogError("敌人状态 要执行的转换状态为空" + CurrentState);
            return;
        }

        foreach (EnemyFSMState fsmState in _states)
        {
            if (fsmState.StateID == nextStateID)
            {
                _currentState.DoBeforeLeaving();
                //Debug.Log("状态转出"+_currentState.StateID);
                _currentState = fsmState;
                //Debug.Log("状态转入"+_currentState.StateID);
                _currentState.DoBeforeEntering();
                return;
            }
        }
    }
}
