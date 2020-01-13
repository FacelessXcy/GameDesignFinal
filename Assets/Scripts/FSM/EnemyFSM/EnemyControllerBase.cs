using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum MonsterType
{
    SmallMelee,
    SmallRange,
    BigMelee,
    BigRange
}

public class EnemyControllerBase : MonoBehaviour
{
    [HideInInspector] public bool isBoomMonster;
    [HideInInspector] public bool gameIsPaused;
    public bool isMeleeMonster;
    public float viewDistance;
    [Range(0,90)]
    public float viewAngle;
    
    public float walkSpeed;

    public int atkDamage;
    public float attackRange;
    public Animator _animator;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    public AudioClip footStep;
    [Header("远程怪物")]
    public GameObject projectilePrefab;
    public GameObject explosionPrefab;
    private MeleeWeaponCollider _meleeWeaponCollider;
    private AudioSource _audioSource;
    private EnemyFSMSystem _fsm;
    [HideInInspector]
    public Rigidbody rigidbody;

    [HideInInspector] public CharacterController characterController;
    private Transform _player;
    public Transform Player => _player;

    private int _runID = Animator.StringToHash("Giant Run");
    private int _takeDamadgeID = Animator.StringToHash("Take Damage");
    private int _deadID = Animator.StringToHash("Die");
    private int _relaxID = Animator.StringToHash("Relax");
    
    //MeleeAttack
    private int _attackID1 =
        Animator.StringToHash("Melee Right Attack 01");
    private int _attackID2 =
        Animator.StringToHash("Melee Right Attack 02");
    private int _attackID3 =
        Animator.StringToHash("Melee Right Attack 03");

    private int[] _attackIDs;
    
    //RangeedAttack
    private int _rangedAttackID =
        Animator.StringToHash("Bow Left Shoot Attack 01");
    [Header("测试属性")]
    //Bool Signal
    //[HideInInspector]
    public bool isInFightBool;
    //[HideInInspector] 
    public bool isBeCarefulBool;
    //[HideInInspector]
    public bool isInDefend;//是否处于霸体状态
    //Trigger Signal
    [HideInInspector]
    public bool isTakeDamadgeTrigger;

    [HideInInspector] public bool isRespawn;
    [HideInInspector]
    public bool isDeadTrigger;
    //Animation Signal
    public bool Idle;
    public bool Attack;
    public bool TakeDamadge;
    public bool isDead;
    private Transform _attackPoint;
    private Health _health;
    private float _takeDamageAnim;//播放受伤动画的最低承受伤害量
    private float _currentTakeDamage=0.0f;
    private RaycastHit _viewHit;
    private int _viewLayerMask;
    private Projectile _currentProjectile;
    public List<GameObject> _allHaveExplosion;
    public GameObject _currentExplosion;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        rigidbody = GetComponent<Rigidbody>();
        _allHaveExplosion=new List<GameObject>();
        _meleeWeaponCollider = transform.Find("WeaponCollider")
        .GetComponent<MeleeWeaponCollider>();
        _attackPoint = transform.Find("AttackPoint");
        _audioSource = GetComponent<AudioSource>();
        _health = GetComponent<Health>();
        if (_animator==null)
        {
            //Debug.Log("_animator==null in Start",gameObject);
            _animator = GetComponent<Animator>();
        }

        if (navMeshAgent==null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        navMeshAgent.ResetPath();
        navMeshAgent.speed = walkSpeed;
        navMeshAgent.stoppingDistance = attackRange/1.2f;
        _attackIDs = new[] {_attackID1, _attackID2, _attackID3};
        _player = GameObject.FindWithTag("Player").transform;
        _health.onDied += onDied;
        _health.onDamaged += OnDamaged;
        _takeDamageAnim = _health.maxHealth / 4;
        _currentTakeDamage = _takeDamageAnim - 0.1f;
        if (_fsm==null)
        {
            MakeFSM(); 
        }
        _viewLayerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
    }


    private void Update()
    {
        if (gameIsPaused)
        {
            return;
        }
        AnimationCheck();
//        if (_fsm.enabled)
//        {
//            Debug.Log("当前状态"+_fsm.CurrentState,gameObject);
//        }
        _fsm.Update(_player);
    }

    public virtual void MakeFSM()
    {
        _fsm=new EnemyFSMSystem(this);
        SmallEnemyIdleState smallEnemyIdle=new SmallEnemyIdleState
        (_fsm);
        smallEnemyIdle.AddTransition(Transition.IdleToChase,
            StateID.SmallChase);
        smallEnemyIdle.AddTransition(Transition.IdleToTakeDamadge,
        StateID.SmallTakeDamadge);
        smallEnemyIdle.AddTransition(Transition.IdleToRelax,StateID.SmallRelax);
        smallEnemyIdle.AddTransition(Transition.IdleToDead,StateID.SmallDead);
        SmallEnemyChaseState smallEnemyChase=new SmallEnemyChaseState
        (_fsm);
        smallEnemyChase.AddTransition(Transition.ChaseToIdle,
        StateID.SmallIdle);
        smallEnemyChase.AddTransition(Transition.ChaseToAttack,
        StateID.SmallAttack);
        smallEnemyChase.AddTransition(Transition.ChaseToTakeDamadge,
        StateID.SmallTakeDamadge);
        smallEnemyChase.AddTransition(Transition.ChaseToDead,StateID
        .SmallDead);
        SmallEnemyRelaxState smallEnemyRelax=new SmallEnemyRelaxState
        (_fsm);
        smallEnemyRelax.AddTransition(Transition.RelaxToIdle,StateID.SmallIdle);
        smallEnemyRelax.AddTransition(Transition.RelaxToTakeDamadge, StateID.SmallTakeDamadge);
        smallEnemyRelax.AddTransition(Transition.RelaxToDead,StateID.SmallDead);
        SmallEnemyAttackState smallEnemyAttack=new SmallEnemyAttackState(_fsm);
        smallEnemyAttack.AddTransition(Transition.AttackToIdle,
        StateID.SmallIdle);
        smallEnemyAttack.AddTransition(Transition.AttackToDead,
        StateID.SmallDead);
        smallEnemyAttack.AddTransition(Transition
        .AttackToTakeDamadge,StateID.SmallTakeDamadge);
        SmallEnemyTakeDamageState smallEnemyTakeDamage=new 
        SmallEnemyTakeDamageState(_fsm);
        smallEnemyTakeDamage.AddTransition(
            Transition.TakeDamadgeToIdle, StateID.SmallIdle);
        SmallEnemyDeadState smallEnemyDead=new SmallEnemyDeadState(_fsm);
        smallEnemyDead.AddTransition(Transition.DeadToIdle,StateID.SmallIdle);
        if (_animator==null)
        {
            _animator = GetComponent<Animator>();
        }

        if (isBoomMonster)
        {
            _fsm.AddStates(smallEnemyIdle,smallEnemyChase,
                smallEnemyAttack,smallEnemyTakeDamage,smallEnemyDead);
        }
        else
        {
            _fsm.AddStates(smallEnemyRelax,smallEnemyIdle,smallEnemyChase,
                smallEnemyAttack,smallEnemyTakeDamage,smallEnemyDead);
        }
        _fsm.AddStates(smallEnemyRelax);

    }
    
    public Vector3 GetVelDir()
    {
        
        return new Vector3(navMeshAgent.velocity.normalized.x,0,
        navMeshAgent.velocity.normalized.z);
    }

    public void SetChaseBool(bool value)
    {
        _animator.SetBool(_runID,value);
    }

    public void SetTakeDamadgeTrigger()
    {
        _animator.SetTrigger(_takeDamadgeID);   
    }

    public void SetAttackTrigger()
    {
        if (isMeleeMonster)
        {
            _animator.SetTrigger(_attackIDs[Random.Range(0,3)]);
        }
        else
        {
            _animator.SetTrigger(_rangedAttackID);
        }
    }
    

    public void SetDeadTrigger()
    {
        _animator.SetTrigger(_deadID);
    }

    public void SetRelaxBool(bool value)
    {
        _animator.SetBool(_relaxID,value);
    }

    private void AnimationCheck()
    {

        Idle = _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        Attack = _animator.GetCurrentAnimatorStateInfo(0)
            .IsTag("Attack");
        TakeDamadge = _animator.GetCurrentAnimatorStateInfo(0)
            .IsName("Take Damage");
    }

    public bool SeePlayer(Vector3 target)
    {
        if (isBoomMonster)
        {
            return true;
        }
        if (Vector3.Angle(transform.forward,target-transform.position)
        <=viewAngle)
        {
            if (Physics.Raycast(transform.position+transform.up.normalized*0.5f,
                target-transform.position,
                out _viewHit, viewDistance,_viewLayerMask))
            {
                if (_viewHit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool FeelPlayer(Vector3 target)
    {
        if (isBoomMonster)
        {
            return true;
        }
        if (Vector3.Distance(target,transform.position)<=3*viewDistance)
        {
            return true;
        }
        return false;
    }

    public bool InTheRound(Vector3 target)
    {
        if (Vector3.Distance(target,transform.position)<=0.5f*viewDistance)
        {
            return true;
        }
        return false;
    }

    public bool IsInAttackRange(Vector3 target)
    {
        if (Vector3.Angle(transform.forward,target-transform.position)
            <=viewAngle)
        {
            if (Vector3.Distance(target,transform.position)<=attackRange)
            {
                return true;
            }
        }
        return false;
    }

    public bool BuildingInAttackRange(Vector3 target)
    {
        RaycastHit hit;
        Debug.DrawRay(_attackPoint.position,
            (target-_attackPoint.position)*attackRange);
        if (Physics.Raycast(_attackPoint.position,
        target-_attackPoint.position,out hit,
        attackRange,(1<<LayerMask.NameToLayer("Building"))|(1<<LayerMask
        .NameToLayer("Wall")))) 
        {
            Debug.Log(hit.collider.name);
            return true;
        }
        return false;
    }


    private void OnDestroy()
    {
        foreach (GameObject o in _allHaveExplosion)
        {
            Destroy(o.gameObject);
        }
    }

    public void onDied()
    {
        Dead();
        if (isBoomMonster)
        {
            EnemyManager.Instance.Recycle(this);
        }
        else
        {
            Destroy(gameObject,3);
        }
    }

    public void OnDamaged(int realDamageAmount,GameObject damageSource)
    {
        if (damageSource.GetComponent<Weapon>().isMeleeWeapon)
        {
            //Debug.Log("isTakeDamadgeTrigger");
            isTakeDamadgeTrigger = true;
        }
        if (!isInDefend)
        {
            _currentTakeDamage += realDamageAmount;
            //Debug.Log(_currentTakeDamage+"  _currentTakeDamage");
            if (_currentTakeDamage>=_takeDamageAnim)
            {
                isTakeDamadgeTrigger = true;
                _currentTakeDamage = 0.0f;
            }
        }
        //Todo
        //更新UI
    }

    public void ResetCurrentTakeDamage()
    {
        _currentTakeDamage = _takeDamageAnim - 0.1f;
    }

    //动画事件
    public void AttackEvent()
    {
        if (isMeleeMonster)//近战攻击
        {
            _meleeWeaponCollider.damage = atkDamage;
            _meleeWeaponCollider.sourceWeapon = this.gameObject;
            if (IsInAttackRange(_player.position))
            {
                if (BuildingInAttackRange(_player.position))
                {
                    //Debug.Log("AttackBuilding");
                    AttackBuilding();
                }
                else
                {
                    //Debug.Log("AttackPlayer");
                    AttackPlayer();
                }
            }
        }
        else
        {
            _currentProjectile=Instantiate(projectilePrefab, _attackPoint.position,
                    Quaternion.LookRotation((_player.position+_player
                    .up*1.2f)
                    -_attackPoint.position)).GetComponent<Projectile>();
            _currentProjectile.explosionPrefab=explosionPrefab;
            _currentProjectile.transform.localScale *= transform
            .localScale.x;
            _currentProjectile.source = this.gameObject;
        }
    }
    
    public void AddCurrentHaveExplosion(GameObject go)
    {
        _allHaveExplosion.Add(go);
        if (_currentExplosion==null)
        {
            _currentExplosion = go;
        }
        go.SetActive(false);
    }
    
    public GameObject GetCurrentExplosion()
    {
        if (_currentExplosion!=null&&!_currentExplosion.activeInHierarchy)
        {
            return _currentExplosion;
        }
        foreach (GameObject o in _allHaveExplosion)
        {
//            if (o==null)
//            {
//                return null;
//            }
            if (!o.activeInHierarchy) 
            {
                _currentExplosion = o;
                return _currentExplosion;
            }
        }
        return null;
    }

    public void AttackBuilding()
    {
        _meleeWeaponCollider.Check(true);
    }

    public void AttackPlayer()
    {
        _player.GetComponent<Damageable>().GetDamage(atkDamage,this.gameObject);
    }

    public void CloseAttackBuilding()
    {
        _meleeWeaponCollider.StopCheck();
    }

    public void FootStep()
    {
        _audioSource.PlayOneShot(footStep);
    }

    public void Respawn()
    {
        if (isDead)
        {
            
            isRespawn = true;
            isDead = false;
            isDeadTrigger = false;
            characterController.enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
            _fsm.enabled = true;
            isBoomMonster = true;
            navMeshAgent.isStopped = false;
            if (_fsm==null)
            {
                MakeFSM(); 
            }
            GetComponent<Health>().Respawn();
            _animator.SetTrigger("Respawn");
        }
    }

    private void Dead()
    {
        isDead = true;
        isDeadTrigger = true;
        GetComponent<CapsuleCollider>().enabled = false;
        if (characterController==null)
        {
            characterController = GetComponent<CharacterController>();
        }
        characterController.enabled = false;
        if (navMeshAgent==null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        if (navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.isStopped = true;
        }
    }

}
