using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallEnemyTakeDamageState : EnemyFSMState
{
    private float _elapsedTime=0.0f;
    private float _duration=0.5f;
    public SmallEnemyTakeDamageState(EnemyFSMSystem fsm) : base(fsm)
    {
        _stateID = StateID.SmallTakeDamadge;
    }

    public override void DoBeforeEntering()
    {
        _elapsedTime = 0.0f;
        _Fsm.character.SetTakeDamadgeTrigger();
        _Fsm.character.navMeshAgent.isStopped = true;
        _Fsm.character.isInFightBool = true;
        _Fsm.character.isBeCarefulBool = true;
    }

    public override void DoBeforeLeaving()
    {
        _Fsm.character.isTakeDamadgeTrigger = false;
    }

    public override void Reason(Transform player)
    {
        _elapsedTime += Time.deltaTime;//动画过渡时间
        if (_elapsedTime<=_duration)
        {
            return;
        }
        if (!_Fsm.character.TakeDamadge)
        {
            _Fsm.PerformTransition(Transition.TakeDamadgeToIdle);
        }
    }

    public override void Act(Transform player)
    {
        
    }
}
