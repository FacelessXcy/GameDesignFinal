using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoSingleton<SceneLoadingManager>
{
    
    private string _targetSceneName;

    public string TargetSceneName => _targetSceneName;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    
    public void LoadNewScene(string sceneName)
    {
        _targetSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
    
}
