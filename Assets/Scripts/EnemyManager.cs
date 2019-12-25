using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    
    private List<EnemyControllerBase> _roadEnemys=new List<EnemyControllerBase>();
    private List<EnemyControllerBase> _boomEnemys=new List<EnemyControllerBase>();


    

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
