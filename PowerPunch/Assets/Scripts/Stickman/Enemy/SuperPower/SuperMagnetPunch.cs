using System;
using UnityEngine;

[CreateAssetMenu]
public class SuperMagnetPunch : SuperPower
{
    [SerializeField] private float _minDistance;
    [SerializeField] private float _radius;
    [SerializeField] private float _activeTime;

    [SerializeField] private AnimationClip _addictionalAnimation;

    private Rigidbody _rigidBody;
    
    private GameObject _player;
    private GameObject _current;
    
    public static Action<bool> ActivateMagnetingForce;
    public static Action<GameObject> ActivateMagnetingParticles;

    public override void Init(GameObject player, GameObject playerRoot, float levelRatio)
    {
        _player = player;
        _rigidBody = playerRoot.GetComponentInChildren<Rigidbody>();
        
        base.Init(player, playerRoot,levelRatio);
        
        ActivateMagnetingParticles?.Invoke(_player);  // активируем партиклы исходящие от игрока
        
        ActivateMagnetingForce.Invoke(true); // активируем притягивание к врагу
    }

    public override void DO()
    {
        EnemyBoxer.LookAtTarget();

        _activeTime -= Time.deltaTime; 

        if (_activeTime <= 0) // если время закончилось, то завершаем все
        {
            ActivateMagnetingForce.Invoke(false);
            
            ActivateMagnetingParticles?.Invoke(null);
            Destroy(_currentParticles);
            
            OverSuperPower();
        }
        else if ((_player.transform.position - EnemyBoxer.transform.position).magnitude <= _minDistance) // если игрок достаточно близко, то начинаем анимацию атаки
        {
            ActivateMagnetingForce.Invoke(false);
            
            ActivateMagnetingParticles?.Invoke(null); // отключаем партиклы и силу притягивния
            Destroy(_currentParticles);

            _myAnimator.Play(_addictionalAnimation.name, 0);
        }
    }
    
    public override void Hitting()
    {
        SetForceToPlayer(_rigidBody, _interractForce, EnemyBoxer.transform.position, _radius); // включаем рэг долл и придаем ему силу

        _playerHealth.TakeDamage(_damage); // игрок получает урон

        OverSuperPower(); // завершаем супер силу
    }
}
