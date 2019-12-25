using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;


    public virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void OnEnter()
    {
        if (canvasGroup==null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public virtual void OnPaused()
    {
        
    }

    public virtual void OnResume()
    {
        
    }
    
    public virtual void OnExit()
    {
        
    }


}
