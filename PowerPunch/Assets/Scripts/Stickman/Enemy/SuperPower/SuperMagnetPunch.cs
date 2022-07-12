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
        
        ActivateMagnetingParticles?.Invoke(_player);

        ActivateMagnetingForce.Invoke(true);
    }

    public override void DO()
    {
        EnemyBoxer.LookAtTarget();

        _activeTime -= Time.deltaTime; 

        if (_activeTime <= 0)
        {
            ActivateMagnetingForce.Invoke(false);
            
            ActivateMagnetingParticles?.Invoke(null);
            Destroy(_currentParticles);
            
            OverSuperPower();
        }
        else if ((_player.transform.position - EnemyBoxer.transform.position).magnitude <= _minDistance)
        {
            ActivateMagnetingForce.Invoke(false);
            
            ActivateMagnetingParticles?.Invoke(null);
            Destroy(_currentParticles);

            _myAnimator.Play(_addictionalAnimation.name, 0);
        }
    }
    
    public override void Hitting()
    {
        SetForceToPlayer(_rigidBody, _interractForce, EnemyBoxer.transform.position, _radius);

        _playerHealth.TakeDamage(_damage);

        OverSuperPower();
    }
}
