using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStory:MonoSingleton<GameStory>
{
    //当前游戏进程Index
    protected int _currentState;

    public override void Awake()
    {
        _destoryOnLoad = true;
        base.Awake();
    }

    public virtual void UpdateStoryState(int stateIndex=-1)
    {
        
    }

}
   