using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xcy.Utility;
using Random = System.Random;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    public Transform currentInstantiatePos;
    
    public MonsterResource monsterResource;
    //资源存储
    private List<EnemyControllerBase> _bigMelee=new  List<EnemyControllerBase>();
    private List<EnemyControllerBase> _bigRange=new  
    List<EnemyControllerBase>();
    private List<EnemyControllerBase> _smallMelee=new  
    List<EnemyControllerBase>();
    private List<EnemyControllerBase> _smallRange=new  
        List<EnemyControllerBase>();
    
    private List<EnemyControllerBase> _roadEnemys=new List<EnemyControllerBase>();
    private List<EnemyControllerBase> _boomEnemys=new List<EnemyControllerBase>();

    private SimpleObjectPool<EnemyControllerBase> _boomEnemysPool;

    private int _enemyCount = 0;
    private void Start()
    {
        LoadEnemyResource();
        _boomEnemysPool=new SimpleObjectPool<EnemyControllerBase>
        (FactoryMethod,null,30);
        for (int i = 0; i < 30; i++)
        {
            AllocateMonster();
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
        _boomEnemys.Add(temp);
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
        temp.gameObject.SetActive(true);
        temp.Respawn();
//        Instantiate(_smallMelee[UnityEngine.Random.Range
//                (0,_smallRange.Count)], 
//            currentInstantiatePos.position+new Vector3(UnityEngine.Random.Range(-3,4),
//                UnityEngine.Random.Range(-3,4),
//                UnityEngine.Random.Range(-3,4)),
//            Quaternion.identity, transform);
    }

    public void Recycle(EnemyControllerBase controllerBase)
    {
        AllocateMonster();
        StartCoroutine(RecycleIEnu(controllerBase));
        
//        Destroy(controllerBase.gameObject,5.0f);
    }

    IEnumerator RecycleIEnu(EnemyControllerBase controllerBase)
    {
        yield return new  WaitForSeconds(3f);
        _boomEnemysPool.Recycle(controllerBase);
        _boomEnemys.Remove(controllerBase);
        controllerBase.gameObject.SetActive(false);
    }

    private void LoadEnemyResource()
    {
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
