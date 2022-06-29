using System;
using BattleVariables;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHP;

    private float _currentHP;
    private float _levelRatio = 1;

    public static Action Damaging;

    private void Awake()
    {
        _currentHP = _maxHP * _levelRatio;
    }

    public float health
    {
        get => _currentHP;
        set => _currentHP = value;
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= damage;
        Damaging.Invoke();
    }

    public float ConvertHP()
    {
        return _currentHP / (_maxHP * _levelRatio);
    }
}
