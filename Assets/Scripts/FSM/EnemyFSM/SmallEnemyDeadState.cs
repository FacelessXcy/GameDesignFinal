using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallEnemyDeadState : EnemyFSMState
{
    public SmallEnemyDeadState(EnemyFSMSystem fsm) : base(fsm)
    {
        _stateID = StateID.SmallDead;
    }

    public override void DoBeforeEntering()
    {
        _Fsm.character.navMeshAgent.destination = _Fsm.character
            .transform.position;
        _Fsm.character.SetDeadTrigger();
        _Fsm.enabled = false;
        _Fsm.character.isDeadTrigger = false;
    }

    public override void DoBeforeLeaving()
    {
        _Fsm.character.isRespawn = false;
    }

    public override void Reason(Transform player)
    {
        if (_Fsm.character.isRespawn)
        {
            _Fsm.PerformTransition(Transition.DeadToIdle);
        }
    }

    public override void Act(Transform player)
    {
        
    }
}
