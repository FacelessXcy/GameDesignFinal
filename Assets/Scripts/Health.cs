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
    
    public int currentHealth;
    public int maxHealth;
    
    public UnityAction<int, GameObject> onDamaged;
    public UnityAction<int> onHealed;
    public UnityAction onDied;
    
    private int _healthBeforeHeal;
    private int _realHealingAmount;

    private int _healBeforeDamage;
    private int _realDamageAmount;
    
    private bool _isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Heal(int healingAmount)
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

    public void TakeDamage(int damage,GameObject damageResource)
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

    public void Kill()
    {
        if (_isDead)
        {
            return;
        }
        currentHealth = 0;
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

    public void Respawn()
    {
        _isDead = false;
        currentHealth = maxHealth;
    }
}
