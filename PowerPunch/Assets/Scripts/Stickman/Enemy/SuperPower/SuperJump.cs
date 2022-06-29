using System;
using System.Collections.Generic;
using BattleVariables;
using UnityEngine;

[CreateAssetMenu]
public class SuperJump : SuperPower
{
    [SerializeField] private float _radius;
    [SerializeField] private float _angle;
    
    private Rigidbody _rigidBody;
    
    private GameObject _player;
    private GameObject _current;
    
    public static Action<bool, float> UiDangerZone;

    public override void Init(GameObject player, GameObject playerRoot, float levelRatio)
    {
        _player = player;
        _rigidBody = playerRoot.GetComponentInChildren<Rigidbody>();
        
        base.Init(player, playerRoot,levelRatio);
        
        UiDangerZone?.Invoke(true, _angle); // включаем и оптравляем радиусу действия удара угол
    }

    public override void DO() {    }

    protected override void GameOver(string tag)    // тут не до конца наладил систему смерти и завершения игры, так как понял, что это не требуется?
    {
        base.GameOver(tag);
        
        UiDangerZone?.Invoke(false, _angle); // выключаем радиус действия удара
    }


    public override void Hitting()
    {
        if ((_player.transform.position - EnemyBoxer.transform.position).magnitude <= _radius)  // смотрим есть ли игрок в радиусе
        {
            SetForceToPlayer(_rigidBody, _interractForce, EnemyBoxer.transform.position, _radius); // включаем рэг долл и придаем силу игроку
            
            _playerHealth.TakeDamage(_damage); // наносим урон игроку
        }

        base.Hitting();  // там мы отключаем партиклы, которые остались
        
        OverSuperPower(); // завершаем действие суперсилы
        
        UiDangerZone?.Invoke(false, _angle); // выключаем радиус действия удара
    }
}
