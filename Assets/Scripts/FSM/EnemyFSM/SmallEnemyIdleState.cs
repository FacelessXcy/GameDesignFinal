using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallEnemyIdleState : EnemyFSMState
{
    private float _duration = 5.0f;
    private float _elapsedTime=0.0f;
    public SmallEnemyIdleState(EnemyFSMSystem fsm) : base(fsm)
    {
        _stateID = StateID.SmallIdle;
    }

    public override void DoBeforeEntering()
    {
        _Fsm.character.isBeCarefulBool = true;
        _Fsm.character.navMeshAgent.isStopped = true;
    }

    public override void DoBeforeLeaving()
    {
        _elapsedTime = 0.0f;
    }

    public override void Reason(Transform player)
    {
        _elapsedTime += Time.deltaTime;
        if (_Fsm.character.isDeadTrigger)//isDeadTrigger死亡时改变
        {
            _Fsm.character.isDeadTrigger = false;
            _Fsm.PerformTransition(Transition.IdleToDead);
            return;
        }
        if (_Fsm.character.isTakeDamadgeTrigger)//isTakeDamadgeTrigger收到攻击时改变
        {
            _Fsm.character.isTakeDamadgeTrigger = false;
            _Fsm.PerformTransition(Transition.IdleToTakeDamadge);
            return;
        }
        if (_Fsm.character.isInFightBool)
        {
            //Debug.Log("IsInFight",_Fsm.character.gameObject);
            _Fsm.PerformTransition(Transition.IdleToChase);
            return;
        }
        if (_Fsm.character.SeePlayer(player.position))
        {
            _Fsm.character.isInFightBool = true;
            PlayerManager.Instance.InBattle = true;
            _Fsm.character.isBeCarefulBool = true;
            _Fsm.PerformTransition(Transition.IdleToChase);
            return;
        }

        _Fsm.character.isInFightBool = false;
        _Fsm.character.ResetCurrentTakeDamage();
        if (_elapsedTime>=_duration)
        {
            _Fsm.character.isBeCarefulBool = false;
            _Fsm.PerformTransition(Transition.IdleToRelax);
        }

    }

    public override void Act(Transform player)
    {
        
    }
}
