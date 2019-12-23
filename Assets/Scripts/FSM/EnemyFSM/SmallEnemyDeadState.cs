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
    }

    public override void DoBeforeLeaving()
    {
        
    }

    public override void Reason(Transform player)
    {
        
    }

    public override void Act(Transform player)
    {
        
    }
}
