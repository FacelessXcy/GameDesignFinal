using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum GameState
{
    Play,
    Pause,
    Stop
}

public enum DifficultMode
{
    Easy,
    Hard
}

public class GameManager : MonoSingleton<GameManager>
{
    [HideInInspector] public bool gameIsPaused;

    [HideInInspector] public string missionDescri =
        "你是一个神秘组织的特工，奉命乘车去一个地区调查那里的奇怪事件。当你到达所在地区卫星城的大桥时，大桥突然坍塌。总部又无法排出支援直升机援助你，你只能靠自己行动了……";

    [HideInInspector] public string endlessDescri =
        "无尽模式：你将面临各种怪物一波一波如潮水般的攻击。活下去，是你唯一的任务。";

     public DifficultMode difficult;
    [HideInInspector] public string[] randomDescri;

    public override void Awake()
    {
        base.Awake();
        randomDescri = new[]
        {
            "如果从过高的地方跳下来，你会直接死亡。",
            "不要轻易的接触该地区中的水，据资料显示，水中含有某种剧毒物质。",
            "每把枪的扩散范围都不同。但所有武器连续射击时的前5发都是比较精准的。" +
            "射击远距离目标时，多尝试短点射。",
            "远程怪物会发射爆炸火箭弹，应尽快处理掉他们。"
        };
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.Instance.gameIsPaused = true;
        PlayerInput.Instance.gameIsPaused = true;
        PlayerWeaponController.Instance.gameIsPaused = true;
        PlayerMover.Instance.gameIsPaused = true;
        BuildingSystem.Instance.gameIsPaused = true;
        EnemyManager.Instance.PauseSystem();
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerManager.Instance.gameIsPaused = false;
        PlayerInput.Instance.gameIsPaused = false;
        PlayerWeaponController.Instance.gameIsPaused = false;
        PlayerMover.Instance.gameIsPaused = false;
        BuildingSystem.Instance.gameIsPaused = false;
        EnemyManager.Instance.ResumeSystem();
        Time.timeScale = 1;
    }



}
