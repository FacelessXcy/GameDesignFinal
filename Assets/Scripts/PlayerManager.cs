using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [HideInInspector]
    public bool gameIsPaused;
    
    public AudioClip takeDamageClip;
    public AudioClip deadClip;
    
    private bool _buildSystemIsOpen = false;
    private bool _weaponSystemIsOpen = false;
    private bool hasInit = false;
    private Health _health;
    private AudioSource _audioSource;
    public bool InBattle { get; set; }

    private float _exitBattleTime;

    private void Start()
    {
        _health = GetComponent<Health>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = takeDamageClip;
        _health.onDamaged += TakeDamage;
        _health.onDied += OnDie;
        _health.onHealed += OnHeal;
        UIManager.Instance.SetHealthText(_health.currentHealth,
        _health.maxHealth);
    }

    private void Update()
    {
        if (gameIsPaused)
        {
            return;
        }
        if (!hasInit)
        {
            CloseBuildSystem();
            OpenWeaponSystem();
            hasInit = true;
        }

        if (InBattle)
        {
            _exitBattleTime += Time.deltaTime;
             
        }
        if (PlayerInput.Instance.switchBuildSystem)
        {
            if (_buildSystemIsOpen)
            {
                CloseBuildSystem();
                OpenWeaponSystem();
                Debug.Log("CloseBuildSystem");
                _buildSystemIsOpen = false;
                _weaponSystemIsOpen = true;
            }
            else
            {
                OpenBuildSystem();
                CloseWeaponSystem();
                Debug.Log("OpenBuildSystem");
                _buildSystemIsOpen = true;
                _weaponSystemIsOpen = false;
            }
        }
    }

    private void CloseBuildSystem()
    {
        BuildingSystem.Instance.CloseBuildingSystem();
    }

    private void OpenBuildSystem()
    {
        BuildingSystem.Instance.OpenBuildingSystem();
    }
    
    private void CloseWeaponSystem()
    {
        PlayerWeaponController.Instance.CloseWeaponSystem();
    }

    private void OpenWeaponSystem()
    {
        PlayerWeaponController.Instance.OpenWeaponSystem();
    }
    
    //受打击
    private void TakeDamage(int f,GameObject g)
    {
        if (GameManager.Instance.difficult==DifficultMode.Easy)
        {
            _health.Heal(100);
        }
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
        UIManager.Instance.SetHealthText(_health.currentHealth,_health.maxHealth);
    }

    private void OnHeal(int a)
    {
        UIManager.Instance.SetHealthText(_health.currentHealth,_health.maxHealth);
    }

    private void OnDie()
    {
        _audioSource.clip = deadClip;
        _audioSource.Play();
        UIManager.Instance.SetHealthText(0,_health.maxHealth);
        //Todo
        if (GameManager.Instance.difficult!=DifficultMode.Easy)
        {
            GameManager.Instance.PauseGame();
            UIManager.Instance.PushPanel(UIType.EndMenuUI);
        }
    }

}
