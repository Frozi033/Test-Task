using System;
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
        
        UiDangerZone?.Invoke(true, _angle);
    }

    public override void DO() {    }

    protected override void GameOver(string tag)
    {
        base.GameOver(tag);
        
        UiDangerZone?.Invoke(false, _angle);
    }


    public override void Hitting()
    {
        if ((_player.transform.position - EnemyBoxer.transform.position).magnitude <= _radius)
        {
            SetForceToPlayer(_rigidBody, _interractForce, EnemyBoxer.transform.position, _radius);

            _playerHealth.TakeDamage(_damage);
        }

        base.Hitting();

        OverSuperPower();

        UiDangerZone?.Invoke(false, _angle);
    }
}
