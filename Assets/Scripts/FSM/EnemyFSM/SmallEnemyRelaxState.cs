using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallEnemyRelaxState : EnemyFSMState
{
    public SmallEnemyRelaxState(EnemyFSMSystem fsm) : base(fsm)
    {
        _stateID = StateID.SmallRelax;
    }

    public override void DoBeforeEntering()
    {
        _Fsm.character.SetRelaxBool(true);
    }

    public override void DoBeforeLeaving()
    {
        _Fsm.character.SetRelaxBool(false);
    }

    public override void Reason(Transform player)
    {
        if (_Fsm.character.isDeadTrigger)//isDeadTrigger死亡时改变
        {
            _Fsm.character.isDeadTrigger = false;
            _Fsm.PerformTransition(Transition.RelaxToDead);
            return;
        }
        if (_Fsm.character.isTakeDamadgeTrigger)//isTakeDamadgeTrigger收到攻击时改变
        {
            _Fsm.character.isTakeDamadgeTrigger = false;
            _Fsm.PerformTransition(Transition.RelaxToTakeDamadge);
            return;
        }
        if (_Fsm.character.SeePlayer(player.position)||
            _Fsm.character.FeelPlayer(player.position))
        {
            _Fsm.PerformTransition(Transition.RelaxToIdle);
        }
    }

    public override void Act(Transform player)
    {
        
    }
}
