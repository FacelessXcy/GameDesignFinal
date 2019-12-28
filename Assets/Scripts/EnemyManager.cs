using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Xcy.Utility;
using Random = System.Random;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    public Transform currentInstantiatePos;
    
    public MonsterResource monsterResource;
    private MakeMonsterPoint[] _monsterPoints;

    public bool keepMakeMonster;
    //资源存储
    private List<EnemyControllerBase> _bigMelee=new  List<EnemyControllerBase>();
    private List<EnemyControllerBase> _bigRange=new  
    List<EnemyControllerBase>();
    private List<EnemyControllerBase> _smallMelee=new  
    List<EnemyControllerBase>();
    private List<EnemyControllerBase> _smallRange=new  
        List<EnemyControllerBase>();
    
    private List<EnemyControllerBase> _roadEnemys=new List<EnemyControllerBase>();
    private Queue<EnemyControllerBase> _boomEnemys=new 
    Queue<EnemyControllerBase>();

    private SimpleObjectPool<EnemyControllerBase> _boomEnemysPool;

    private int _enemyCount = 0;
    private int _needKillEnemyCount = 0;
    private void Awake()
    {
        _destoryOnLoad = true;
    }

    private void Start()
    {
        if (monsterResource==null)
        {
            Debug.Log("monsterResource 为空，需要赋值");
            return;
        }

        _monsterPoints=GetComponentsInChildren<MakeMonsterPoint>();
        currentInstantiatePos = _monsterPoints[0].transform;
        LoadEnemyResource();
        _boomEnemysPool=new SimpleObjectPool<EnemyControllerBase>
        (FactoryMethod,null,30);
//        for (int i = 0; i < 30; i++)
//        {
//            AllocateMonster();
//        }
    }


    public void MakeMonsterAtPosition(int pointIndex,int 
    productCount,int needKillCount,bool recycleList=true)
    {
        if (recycleList)
        {
            Debug.Log("RecycleMonsterList");
            RecycleMonsterList(false);
        }
        keepMakeMonster = true;
        _needKillEnemyCount = needKillCount;
        currentInstantiatePos = _monsterPoints[pointIndex].transform;
        StartCoroutine(MakeMonsterAtPositionIEnu(productCount));
    }

    IEnumerator MakeMonsterAtPositionIEnu(int productCount)
    {
        for (int i = 0; i < productCount; i++)
        {
            AllocateMonster();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void RecycleMonsterList(bool countKill)
    {
//        int count = _boomEnemys.Count;
//        for (int i = 0; i < count; i++)
//        {
//            Recycle(_boomEnemys.Peek(),true,countKill);
//        }
        int count = _boomEnemys.Count*2/3;
        StartCoroutine(RecycleMonsterListIEnum(count,countKill));
    }

    IEnumerator RecycleMonsterListIEnum(int count,bool countKill)
    {
        for (int i = 0; i < count; i++)
        {
            Recycle(_boomEnemys.Peek(),false,countKill);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private EnemyControllerBase FactoryMethod()
    {
        EnemyControllerBase temp;
        if (_enemyCount%5==0) 
        {
            //每5个近战小怪生成1个远程小怪
            temp=Instantiate(_smallRange[UnityEngine.Random.Range
            (0,_smallRange.Count)], 
            currentInstantiatePos.position,
                Quaternion.identity, transform);

        }else if (_enemyCount%9==0)
        {
            //每9个近战小怪生成1个近战大怪 
            temp=Instantiate(_bigMelee[UnityEngine.Random.Range
                    (0,_bigMelee.Count)], 
                currentInstantiatePos.position,
                Quaternion.identity, transform);
            
        }else if (_enemyCount%6==0)
        {
            //每12个近战小怪生成1个远程大怪 
            temp=Instantiate(_bigRange[UnityEngine.Random.Range
                    (0,_bigRange.Count)], 
                currentInstantiatePos.position,
                Quaternion.identity, transform);
        }else
        {
            //生成近战小怪
            temp=Instantiate(_smallMelee[UnityEngine.Random.Range
                    (0,_smallMelee.Count)], 
                currentInstantiatePos.position,
                Quaternion.identity, transform);
        }
        
        _enemyCount++;
        temp.isBoomMonster = true;
        _boomEnemys.Enqueue(temp);
        temp.gameObject.SetActive(false);
        return temp;
    }

    public void AllocateMonster()
    {
        EnemyControllerBase temp = _boomEnemysPool.Allocate();
        temp.transform.position = currentInstantiatePos.position+
                                  new Vector3(
                                      UnityEngine.Random.Range(-3,4),
                                      UnityEngine.Random.Range(-3,4),
                                      UnityEngine.Random.Range(-3,4));
        if (!_boomEnemys.Contains(temp))
        {
            _boomEnemys.Enqueue(temp);
        }
        temp.gameObject.SetActive(true);
        temp.Respawn();
    }

    public void Recycle(EnemyControllerBase controllerBase,bool 
    immediatelyDelete=false,bool countEnemyKill=true)
    {
        if (!controllerBase.isDead)
        {
            controllerBase.onDied();
        }
        if (controllerBase.isBoomMonster&&keepMakeMonster)
        {
            AllocateMonster();
        }
        if (countEnemyKill)
        {
            _needKillEnemyCount--;
        }
        if (_needKillEnemyCount==0)
        {
            Debug.Log("_needKillEnemyCount==0");
            keepMakeMonster = false;
        }

        if (!immediatelyDelete)
        {
            StartCoroutine(RecycleIEnu(controllerBase));
        }
        else
        {
            _boomEnemysPool.Recycle(controllerBase);
            _boomEnemys.Dequeue();
            controllerBase.gameObject.SetActive(false);
        }

    }

    IEnumerator RecycleIEnu(EnemyControllerBase controllerBase)
    {
        yield return new  WaitForSeconds(3f);
        _boomEnemysPool.Recycle(controllerBase);
        _boomEnemys.Dequeue();
        controllerBase.gameObject.SetActive(false);
    }

    private void LoadEnemyResource()
    {
        if (monsterResource==null)
        {
            Debug.Log("monsterResource 为空，需要赋值");
            return;
        }
        foreach (MonsterLoadInfo loadInfo in monsterResource.monsterPath)
        {
            switch (loadInfo.monsterType)
            {
                case MonsterType.BigMelee:
                    _bigMelee.Add(Resources.Load<EnemyControllerBase>
                    (loadInfo.path));
                    break;
                case MonsterType.BigRange:
                    _bigRange.Add(Resources.Load<EnemyControllerBase>
                        (loadInfo.path));
                    break;
                case MonsterType.SmallMelee:
                    _smallMelee.Add(Resources.Load<EnemyControllerBase>
                        (loadInfo.path));
                    break;
                case MonsterType.SmallRange:
                    _smallRange.Add(Resources.Load<EnemyControllerBase>
                        (loadInfo.path));
                    break;
            }
        }
    }

    public void PauseSystem()
    {
        foreach (EnemyControllerBase enemy in _roadEnemys)
        {
            enemy.gameIsPaused = true;
        }
        foreach (EnemyControllerBase enemy in _boomEnemys)
        {
            enemy.gameIsPaused = true;
        }
    }

    public void ResumeSystem()
    {
        foreach (EnemyControllerBase enemy in _roadEnemys)
        {
            enemy.gameIsPaused = false;
        }
        foreach (EnemyControllerBase enemy in _boomEnemys)
        {
            enemy.gameIsPaused = false;
        }
    }

}
