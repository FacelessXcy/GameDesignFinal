using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmallEnemyChaseState : EnemyFSMState
{
    private float _inAttackTime=0.5f;
    private float _currentInAttackTime;
    private Vector3 tempDir;
    public SmallEnemyChaseState(EnemyFSMSystem fsm) : base(fsm)
    {
        _stateID = StateID.SmallChase;
    }
    
    public override void DoBeforeEntering()
    {
        _Fsm.character.SetChaseBool(true);
        _Fsm.character.navMeshAgent.isStopped = false;
//        _Fsm.character.navMeshAgent.SetDestination(_Fsm.character
//            .Player.position);
    }

    public override void DoBeforeLeaving()
    {
        _Fsm.character.SetChaseBool(false);
    }

    public override void Reason(Transform player)
    {
        //死亡
        if (_Fsm.character.isDeadTrigger)
        {
            _Fsm.character.isDeadTrigger = false;
            _Fsm.PerformTransition(Transition.ChaseToDead);
            return;
        }
        if (_Fsm.character.isTakeDamadgeTrigger)//受伤
        {
            _Fsm.character.isTakeDamadgeTrigger = false;
            _Fsm.PerformTransition(Transition.ChaseToTakeDamadge);
            return;
        }
        //丢失目标
        if (!_Fsm.character.FeelPlayer(player.position)&&
            _Fsm.character.isInFightBool)
        {
            Debug.Log("丢失目标");
            _Fsm.character.isInFightBool = false;
            _Fsm.PerformTransition(Transition.ChaseToIdle);
            return;
        }
        //攻击目标
        if (_Fsm.character.IsInAttackRange(player.position))
        {
            //Debug.Log("防止角色乱闪");
            _currentInAttackTime += Time.deltaTime;//防止角色乱闪
            if (_currentInAttackTime>=_inAttackTime)
            {
                _Fsm.PerformTransition(Transition.ChaseToAttack);
            }
        }
        else
        {
            _currentInAttackTime = 0.0f;
        }
    }

    public override void Act(Transform player)
    {
        //Debug.Log("Set Forward  "+ _Fsm.character.GetVelDir());
        _Fsm.character.navMeshAgent.SetDestination(player.position);
        if (_Fsm.character.navMeshAgent.velocity!=Vector3.zero)
        {
            _Fsm.character.transform.forward = _Fsm.character.GetVelDir();
        }
        else
        {
            tempDir = player.position - _Fsm.character.transform.position;
            _Fsm.character.transform.forward = new Vector3(tempDir.x,
                _Fsm.character.transform.forward.y,tempDir.z);
        }
    }
}
