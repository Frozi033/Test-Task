using System;
using BattleVariables;
using UnityEngine;

public class MyAmmo : MonoBehaviour, IAmmo
{
    [SerializeField] private int _currentAmmo;
    
    public static Action AmmoChange;
    
    public int Ammo
    {
        get => _currentAmmo;
        set => _currentAmmo = value;
    }

    public void AmmoMinus()
    {
        _currentAmmo--;
        AmmoChange.Invoke();
    }
}
