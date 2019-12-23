using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Health : MonoBehaviour
{
    
    public float currentHealth;
    public float maxHealth;
    
    public UnityAction<float, GameObject> onDamaged;
    public UnityAction<float> onHealed;
    public UnityAction onDied;

    private float _healthBeforeHeal;
    private float _realHealingAmount;

    private float _healBeforeDamage;
    private float _realDamageAmount;
    
    private bool _isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Heal(float healingAmount)
    {
        if (_isDead)
        {
            return;
        }
        //Todo
        //更新UI
        _healthBeforeHeal = currentHealth;
        currentHealth += healingAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        _realHealingAmount = currentHealth - _healthBeforeHeal;
        if (onHealed!=null)
        {
            onHealed.Invoke(_realHealingAmount);
        }
    }

    public void TakeDamage(float damage,GameObject damageResource)
    {
        if (_isDead)
        {
            return;
        }
        //Todo
        //更新UI
        _healBeforeDamage = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        _realDamageAmount =_healBeforeDamage- currentHealth ;
        if (onDamaged!=null)
        {
            onDamaged.Invoke(_realDamageAmount,damageResource);
        }
        DeathHandle();
    }

    private void DeathHandle()
    {
        if (currentHealth<=0)
        {
            _isDead = true;
            if (onDied!=null)
            {
                onDied.Invoke();
            }
        }
    }


}
