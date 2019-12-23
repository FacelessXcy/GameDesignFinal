using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VirtualBuilding : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Color _originColor;
    private Color _disableColor;
    private float _minDistance;
    private bool _canBuild;

    public bool CanBuild => _canBuild;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originColor = _meshRenderer.material.color;
        _disableColor=Color.red;
        _minDistance = transform.lossyScale.x/transform.lossyScale.x;
        //Debug.Log(_minDistance);
        _canBuild = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!BuildingSystem.Instance.NextToBuilding)
        {
            if (_meshRenderer.material.color!=_disableColor)
            {
                _meshRenderer.material.color = _disableColor;
                //Debug.Log("_meshRenderer.material.color!=_disableColor");
            }
            _canBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_meshRenderer.material.color==_disableColor)
        {
            _meshRenderer.material.color = _originColor;
        }

        _canBuild = true;
    }
}
