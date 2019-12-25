using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterResourcePath",menuName="MonsterResourcePath")]
public class MonsterResource : ScriptableObject
{

    public List<MonsterLoadInfo> monsterPath;

}

[Serializable]
public class MonsterLoadInfo
{
    public MonsterType monsterType;
    public string path;
}

