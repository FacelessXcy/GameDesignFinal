using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    
    public Slider loadingSlider;
    public Text loadingText;
    private AsyncOperation _loadSceneAO;
    private float _valueTarget;


    void Start()
    {
        loadingSlider.value = 0;
        loadingText.text = (0*100).ToString() + " %";
        if (SceneManager.GetActiveScene().name=="LoadingScene")
        {
            StartCoroutine(AsyncLoading());
        }
        Time.timeScale = 1;
    }

    void Update()
    {
        _valueTarget = _loadSceneAO.progress;
        if (_valueTarget>=0.9f)
        {
            _valueTarget = 1;
        }

        if (_valueTarget!=loadingSlider.value)
        {
            loadingSlider.value = Mathf.Lerp(loadingSlider.value,_valueTarget,Time.deltaTime);
            if (Mathf.Abs(loadingSlider.value-_valueTarget)<0.01f)
            {
                loadingSlider.value = _valueTarget;
            }
        }
        loadingText.text = ((int)(loadingSlider.value*100)).ToString() + " %";
        if (loadingSlider.value==1)
        {
            loadingText.text = "加载完成，按G键进入";
            if (Input.GetKeyDown(KeyCode.G))
            {
                _loadSceneAO.allowSceneActivation = true;
            }
        } 
        
    }
    IEnumerator AsyncLoading()
    {
        _loadSceneAO = SceneManager.LoadSceneAsync(SceneLoadingManager.Instance
        .TargetSceneName);
        _loadSceneAO.allowSceneActivation = false;
        yield return _loadSceneAO;
    }
}
