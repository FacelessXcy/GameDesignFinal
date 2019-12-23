using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraController : MonoBehaviour
{

    private float _xRotation=0;
    

    private void Update()
    {
        _xRotation -= PlayerMover.Instance.MouseVertical;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        transform.localRotation=Quaternion.Euler(_xRotation,0,0);
    }



}
