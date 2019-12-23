using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallEnemyAttackState : EnemyFSMState
{
    private Vector3 tempDir;
    public SmallEnemyAttackState(EnemyFSMSystem fsm) : base(fsm)
    {
        _stateID = StateID.SmallAttack;
    }

    public override void DoBeforeEntering()
    {

    }

    public override void DoBeforeLeaving()
    {
        
    }

    public override void Reason(Transform player)
    {
        if (_Fsm.character.isDeadTrigger)
        {
            _Fsm.PerformTransition(Transition.AttackToDead);
            return;
        }
        if (_Fsm.character.isTakeDamadgeTrigger)//受伤
        {
            _Fsm.character.isTakeDamadgeTrigger = false;
            _Fsm.PerformTransition(Transition.AttackToTakeDamadge);
            return;
        }
        if (!_Fsm.character.IsInAttackRange(player.position)&&!_Fsm.character.Attack)
        {
            //Debug.Log("AttackToIdle");
            _Fsm.PerformTransition(Transition.AttackToIdle);
        }
    }

    public override void Act(Transform player)
    {

        if (!_Fsm.character.Attack)
        {
            _Fsm.character.SetAttackTrigger();
            tempDir = player.position -
                      _Fsm.character.transform.position;
            _Fsm.character.transform.forward = new Vector3(tempDir.x,
                _Fsm.character.transform.forward.y,tempDir.z);
        }
    }
    

}
