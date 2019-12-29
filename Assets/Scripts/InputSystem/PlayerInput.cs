using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInput : MonoSingleton<PlayerInput>
{
    [HideInInspector] public bool gameIsPaused;
    [Header("===== Key Settings=====")]
    public string keyBuild;
    public string keyReload;
    public string keySwitchBuildSystem;

    private MyButton _buttonRun=new MyButton();
    private MyButton _buttonBuild=new MyButton();
    private MyButton _buttonJump=new MyButton();
    private MyButton _buttonFire=new MyButton();
    private MyButton _buttonReload=new MyButton();
    private MyButton _buttonSwitchBuildSystem=new MyButton();
    private MyButton _buttonChangeWeapon1=new MyButton();
    private MyButton _buttonChangeWeapon2=new MyButton();
    private MyButton _buttonChangeWeapon3=new MyButton();
    private MyButton _buttonChangeWeapon4=new MyButton();

    [Header("===== Output Signals=====")] 
    public float keyboardVertical;
    public float keyboardHorizontal;
    public float mouseVertical;
    public float mouseHorizontal;
    
    public bool leftFireDown;
    public bool leftFireHold;
    public bool leftFireUp;
    public bool reload;
    
    public bool jump;
    public bool build;
    public bool switchBuildSystem;
    public bool run;

    public bool changeWeapon1;
    public bool changeWeapon2;
    public bool changeWeapon3;
    public bool changeWeapon4;

    public override void Awake()
    {
        _destoryOnLoad = true;
        base.Awake();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        mouseVertical = 0;
        mouseHorizontal = 0;
    } 

    private void Update()
    {
        if (gameIsPaused)
        {
            ResetSignal();
            return;
        }
        

        _buttonBuild.Tick(Input.GetKey(keyBuild));
        _buttonJump.Tick(Input.GetButton("Jump"));
        _buttonFire.Tick(Input.GetButton("Fire1"));
        _buttonReload.Tick(Input.GetKey(keyReload));
        _buttonSwitchBuildSystem.Tick(Input.GetKey(keySwitchBuildSystem));
        _buttonRun.Tick(Input.GetKey(KeyCode.LeftShift));
        _buttonChangeWeapon1.Tick(Input.GetKey(KeyCode.Alpha1));
        _buttonChangeWeapon2.Tick(Input.GetKey(KeyCode.Alpha2));
        _buttonChangeWeapon3.Tick(Input.GetKey(KeyCode.Alpha3));
        _buttonChangeWeapon4.Tick(Input.GetKey(KeyCode.Alpha4));
        
        keyboardVertical = Input.GetAxis("Vertical");
        keyboardHorizontal = Input.GetAxis("Horizontal");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");


        jump = _buttonJump.onPressed;
        leftFireDown = _buttonFire.onPressed;
        leftFireHold = _buttonFire.isPressing;
        leftFireUp = _buttonFire.onReleased;
        reload = _buttonReload.onPressed;
        build = _buttonBuild.onPressed;
        switchBuildSystem = _buttonSwitchBuildSystem.onPressed;
        run = _buttonRun.isPressing;
        changeWeapon1 = _buttonChangeWeapon1.onPressed;
        changeWeapon2 = _buttonChangeWeapon2.onPressed;
        changeWeapon3 = _buttonChangeWeapon3.onPressed;
        changeWeapon4 = _buttonChangeWeapon4.onPressed;

    }


    private void ResetSignal()
    {
        keyboardVertical = 0;
        keyboardHorizontal = 0;
        mouseHorizontal = 0;
        mouseVertical = 0;
            
        jump = false;
        leftFireDown = false;
        leftFireHold = false;
        leftFireUp = false;
        reload = false;
        build = false;
        switchBuildSystem = false;
        run = false;
        changeWeapon1 = false;
        changeWeapon2 = false;
        changeWeapon3 = false;
        changeWeapon4 = false;
    }

}
