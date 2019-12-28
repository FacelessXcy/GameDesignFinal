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

public class GameManager : MonoSingleton<GameManager>
{
    [HideInInspector] public bool gameIsPaused;

    [HideInInspector] public string missionDescri =
        "你是一个神秘组织的特工，奉命乘车去一个地区调查那里的奇怪事件。当你到达所在地区卫星城的大桥时，大桥突然坍塌。总部又无法排出支援直升机援助你，你只能靠自己行动了……";

    [HideInInspector] public string endlessDescri =
        "无尽模式：你将面临各种怪物一波一波如潮水般的攻击。活下去，是你唯一的任务。";

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
