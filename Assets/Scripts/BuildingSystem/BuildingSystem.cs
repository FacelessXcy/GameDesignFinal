using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum BuildPos
{
    Up,
    Down,
    Right,
    Left,
    Front,
    Back
}

public class BuildingSystem : MonoSingleton<BuildingSystem>
{
    
    public float rotateSpeed;
    [FormerlySerializedAs("virtualBuilding")] public GameObject virtualBuildingGo;
    [FormerlySerializedAs("entityBuilding")] public GameObject entityBuildingGo;
    public float buildDistance;
    [HideInInspector]
    public bool isOpen;
    
    private Ray _ray;
    private RaycastHit _hit;
    private LayerMask _layerMask;
    private Camera _camera;
    private Vector3 _curVirtualPos;//虚拟模型当前位置
    private Quaternion _curVirtualRot;//虚拟模型当前旋转
    private float buildingStep;//建造箱子的间隔距离
    private bool _nextToBuilding;
    private GameObject _curHitGo;
    private VirtualBuilding _virtualBuilding;
    public bool NextToBuilding => _nextToBuilding;
    public GameObject CurrentHitGameObject => _curHitGo;
    
    private void Start()
    {
        _nextToBuilding = false;
        _layerMask = 1 << LayerMask.NameToLayer("Building")
        |1<<LayerMask.NameToLayer("Wall")|1<<LayerMask.NameToLayer
        ("Ground");
        _camera=Camera.main;
        _ray=new Ray(transform.position,transform.forward.normalized);
        buildingStep = virtualBuildingGo.transform.lossyScale.x/virtualBuildingGo.transform.lossyScale.x;
        _virtualBuilding = virtualBuildingGo.GetComponent<VirtualBuilding>();
    }
    

    private void Update()
    {
        if (!isOpen)
        {
            return;
        }
        RayTest();
        Build();
    }


    private BuildPos CalcBuildPosType(Vector3 hitCol,Vector3 hitPoint)
    {
        BuildPos result=default(BuildPos);
        Vector3 tempDir = hitPoint - hitCol;
        float tempDirAbsX = Mathf.Abs(tempDir.x);
        float tempDirAbsY = Mathf.Abs(tempDir.y);
        float tempDirAbsZ = Mathf.Abs(tempDir.z);
        float maxAxis =
            Mathf.Max(tempDirAbsX, tempDirAbsY, tempDirAbsZ);
        if (maxAxis==tempDirAbsX)
        {
            if (tempDir.x>0)
            {
                result = BuildPos.Right;
            }
            else
            {
                result = BuildPos.Left;
            }
        }
        else if (maxAxis==tempDirAbsY)
        {
            if (tempDir.y>0)
            {
                result = BuildPos.Up;
            }
            else
            {
                result = BuildPos.Down;
            }
        }
        else if (maxAxis==tempDirAbsZ)
        {
            if (tempDir.z>0)
            {
                result = BuildPos.Front;
            }
            else
            {
                result = BuildPos.Back;
            }
        }

        return result;

    }

    private void Build()
    {
        if (PlayerInput.Instance.build)
        {
            if (_curHitGo!=null&&_virtualBuilding.CanBuild)
            {
                Instantiate(entityBuildingGo, _curVirtualPos,
                    _curVirtualRot);
            }
            else
            {
                //无法建造提示
                Debug.Log("无法建造");
            }
        }
    }

    private void RayTest()
    {
        _ray.origin = _camera.transform.position;
        _ray.direction = _camera.transform.forward.normalized;
        if (Physics.Raycast(_ray,out _hit,buildDistance,_layerMask))
        {
            if (_hit.collider.gameObject.layer ==
                LayerMask.NameToLayer("Building"))
            {
                
                _curHitGo = _hit.collider.gameObject;
                _curVirtualRot = _hit.collider.transform.rotation;
                switch (CalcBuildPosType(_hit.collider.transform
                    .position, _hit.point))
                {
                    case BuildPos.Front:
                        _curVirtualPos = _hit.collider.transform.position +
                                         _hit.collider.transform.forward*buildingStep;
                        break;
                    case BuildPos.Back:
                        _curVirtualPos = _hit.collider.transform.position +
                                         (-_hit.collider.transform.forward)*buildingStep;
                        break;
                    case BuildPos.Up:
                        _curVirtualPos = _hit.collider.transform.position +
                                         _hit.collider.transform.up*buildingStep;
                        break;
                    case BuildPos.Down:
                        _curVirtualPos = _hit.collider.transform.position +
                                         (-_hit.collider.transform.up)*buildingStep;
                        break;
                    case BuildPos.Right:
                        _curVirtualPos = _hit.collider.transform.position +
                                         _hit.collider.transform.right*buildingStep;
                        break;
                    case BuildPos.Left:
                        _curVirtualPos = _hit.collider.transform.position +
                                         (-_hit.collider.transform.right)*buildingStep;
                        break;
                }

                _nextToBuilding = true;
            }
            else 
            {
                //如果射线检测到Ground Wall
                _curHitGo = _hit.collider.gameObject;
                _curVirtualPos = _hit.point +
                                 _hit.normal * buildingStep /2.0f;
                _nextToBuilding = false;
                if (_hit.collider.gameObject.layer ==
                    LayerMask.NameToLayer("Ground"))
                {
                    _curVirtualRot =
                        virtualBuildingGo.transform.rotation;
                }
                else
                {
                    _curVirtualRot =_curHitGo.transform.rotation;
                }

                
                
            }
        }
        else
        {
            _curVirtualRot =virtualBuildingGo.transform.rotation;
            _curVirtualPos = _camera.transform.position + _camera.transform.forward
                                 .normalized * buildDistance;
            _curHitGo = null;
            _nextToBuilding = false;
        }
        virtualBuildingGo.transform.position = _curVirtualPos;
        virtualBuildingGo.transform.rotation = _curVirtualRot;
        if (!_nextToBuilding)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                virtualBuildingGo.transform.Rotate(Vector3.up,
                rotateSpeed*Time.deltaTime,Space.Self);
            }else if (Input.GetKey(KeyCode.E))
            {
                virtualBuildingGo.transform.Rotate(Vector3.up,
                    -rotateSpeed*Time.deltaTime,Space.Self);
            }
        }
    }

    

    public void CloseBuildingSystem()
    {
        _virtualBuilding.gameObject.SetActive(false);
        isOpen = false;
    }

    public void OpenBuildingSystem()
    {
        _virtualBuilding.gameObject.SetActive(true);
        isOpen = true;
    }

}
