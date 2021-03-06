﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerMover : MonoSingleton<PlayerMover>
{
    [HideInInspector]public bool gameIsPaused;
    
    #region public 
    public float jumpHeight;
    public float walkSpeed;
    public float verticalSensitivity;
    public float horizontalSensitivity;
    public float MouseVertical => _mouseVertical;
    public float MouseHorizontal => _mouseHorizontal;
    public float footClipPlayStep;
    
    public AudioClip[] footStepClips;
    public AudioClip landClip;
    public AudioClip jumpClip;

    [HideInInspector]
    public bool isJump;

    public bool isRun;
    
    #endregion

    
    
    #region private
    private Transform _playerModel;
    private AudioSource _audioSource;
    private float _currentFootStepDis=0.0f;
    private float _mouseVertical;
    private float _mouseHorizontal;
    private float _walkForwardOrBack;
    private float _walkRightOrLeft;
    private Vector3 _tempMoveDir;
    private Vector3 _velY;
    private float _multiSpeed;
    private bool _isGround;
    private bool _lastFrameIsGround;
    private CharacterController _characterController;
    private bool _realInGround;
    private float _inGroundMaxTime=0.2f;
    private float _inGroundCurrentTime;
    private float _lastLeaveGroundMaxY=0.0f;
    #endregion


    public override void Awake()
    {
        _destoryOnLoad = true;
        base.Awake();
    }

    private void Start()
    {
        _audioSource = transform.Find("MoveAudio").GetComponent<AudioSource>();
        _mouseVertical = 0;
        _mouseHorizontal = 0;
        _characterController = GetComponent<CharacterController>();
        _playerModel = transform.Find("Player");

        SetPlayerSetting();
    }

    
    private void Update()
    {
        if (gameIsPaused)
        {
            _mouseVertical = 0;
            _mouseHorizontal = 0;
            return;
        }
        
        _isGround=_characterController.SimpleMove(Vector3.zero);
        if (!_isGround)
        {
            if (_lastLeaveGroundMaxY<=transform.position.y)
            {
                _lastLeaveGroundMaxY = transform.position.y;
            }
            _inGroundCurrentTime += Time.deltaTime;
            if (_inGroundCurrentTime>=_inGroundMaxTime)
            {
                _realInGround = false;
            }
        }
        else
        {
            if (_lastLeaveGroundMaxY-transform.position.y>=5)
            {
                Debug.Log("摔死");
                GetComponent<Health>().Kill();
            }

            _lastLeaveGroundMaxY = transform.position.y;
            _inGroundCurrentTime = 0.0f;
            _realInGround = true;
        }
        
        if (_isGround)
        {
            _velY.y = 0;
        }
        GetInput();
        HandleMove();
        SoundPlay();
        
        _lastFrameIsGround = _realInGround;
    }

    private void GetInput()
    {
        //输入获取
        _mouseHorizontal = PlayerInput.Instance.mouseHorizontal*
                           Time.deltaTime*horizontalSensitivity;
        _mouseVertical = PlayerInput.Instance.mouseVertical*
                         Time.deltaTime*verticalSensitivity;
        _walkRightOrLeft = PlayerInput.Instance.keyboardHorizontal;
        _walkForwardOrBack = PlayerInput.Instance.keyboardVertical;
        isRun = !PlayerInput.Instance.leftFireHold&&
                PlayerInput.Instance.run&&_walkForwardOrBack>0;
    }

    private void HandleMove()
    {
        _multiSpeed = isRun ? 1.5f : 1.0f;
        //Handle旋转与移动
        transform.Rotate(Vector3.up,_mouseHorizontal,Space.World);
        _tempMoveDir = (transform.forward * _walkForwardOrBack + 
                        transform.right * _walkRightOrLeft).normalized;
        //Handle跳跃
        if (_realInGround&&PlayerInput.Instance.jump)
        {
            _velY.y = Mathf.Sqrt(2 * 9.8f * jumpHeight);
            isJump = true;
            _audioSource.PlayOneShot(jumpClip);
        }
        else
        {
            isJump = false;
        }
        _velY.y += -9.8f * Time.deltaTime;
        _characterController.Move(_tempMoveDir *_multiSpeed* walkSpeed *
                                  Time.deltaTime+_velY * Time.deltaTime);
        if (_realInGround)
        {
            //脚步声音循环判定
            if (_walkRightOrLeft!=0||_walkForwardOrBack!=0)
            {
                _currentFootStepDis += walkSpeed*_multiSpeed*Time.deltaTime;
            }
        }
        else
        {
            _currentFootStepDis = 0;
        }
    }

    private void SoundPlay()
    {
        if (_lastFrameIsGround!=_realInGround)
        {
            if (_realInGround)
            {
                _audioSource.PlayOneShot(landClip);
                //Debug.Log("Play landClip");
            }
        }

        if (_realInGround&&_currentFootStepDis>=footClipPlayStep)
        {
            _currentFootStepDis = 0;
            _audioSource.PlayOneShot(footStepClips[Random.Range(0,
            footStepClips.Length)]);
            //Debug.Log("Play footClip");
        }
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.layer==LayerMask.NameToLayer("DeadZone"))
        {
            GetComponent<Health>().Kill();
        }
    }

    public void SetPlayerSetting()
    {
        verticalSensitivity =
            PlayerSetting.Instance.mouseVerticalSensitivity;
        horizontalSensitivity =
            PlayerSetting.Instance.mouseHorizontalSensitivity;
    }

}
