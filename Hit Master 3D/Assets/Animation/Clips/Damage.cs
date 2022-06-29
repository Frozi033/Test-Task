using BattleVariables;
using UnityEngine;

public class Damage : MonoBehaviour, IDamaging
{
    [SerializeField] private float _currentDamage;

    public float damage
    {
        get => _currentDamage;
        
        set => _currentDamage = value;
    }
}
