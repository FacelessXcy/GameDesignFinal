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


    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.Instance.gameIsPaused = true;
        PlayerInput.Instance.gameIsPaused = true;
        EnemyManager.Instance.PauseSystem();
        PlayerWeaponController.Instance.gameIsPaused = true;
        PlayerMover.Instance.gameIsPaused = true;
        BuildingSystem.Instance.gameIsPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerManager.Instance.gameIsPaused = false;
        PlayerInput.Instance.gameIsPaused = false;
        EnemyManager.Instance.ResumeSystem();
        PlayerWeaponController.Instance.gameIsPaused = false;
        PlayerMover.Instance.gameIsPaused = false;
        BuildingSystem.Instance.gameIsPaused = false;
        Time.timeScale = 1;
    }



}
