using System;
using BattleVariables;
using UnityEngine;

public abstract class SuperPower : ScriptableObject
{
    [SerializeField] private Vector3 _particleSystemPos;

    [SerializeField] private AnimationClip _curAnim;
    
    [SerializeField] protected float _interractForce;
    [SerializeField] protected float _damage;

    [SerializeField] protected GameObject _particleSystem;

    // protected float _levelRatio = 1;

    protected GameObject _currentParticles;
    protected Animator _myAnimator;
    protected IDamageable _playerHealth;

    [HideInInspector] public Enemy EnemyBoxer;
    
    public static Action PlayerRBOn;
    public static Action SuperAttackOver;
    

    public virtual void Init(GameObject player, GameObject playerRoot, float levelRatio)
    {
        _myAnimator = EnemyBoxer.GetComponent<Animator>();
        _playerHealth = player.GetComponent<IDamageable>();
        // _levelRatio = levelRatio;

       HPBar.SMBDead += GameOver;
        
        _currentParticles = Instantiate(_particleSystem, _particleSystemPos, EnemyBoxer.transform.rotation, EnemyBoxer.transform);

        _myAnimator.Play(_curAnim.name, 0);
    }

    protected virtual void GameOver(string tag)
    {
        _myAnimator.SetLayerWeight(1, 1);
        EnemyBoxer.CurrentSuperPower = null;
        EnemyBoxer.CurrentState = null;
        Destroy(_currentParticles);
    }
    
    public abstract void DO();

    public void Charging(float speed)
    {
        _myAnimator.speed = speed;
    }

    public virtual void OverSuperPower()
    {
        SuperAttackOver?.Invoke();
    }

    public virtual void Hitting()
    {
        _currentParticles.GetComponentInChildren<ParticleSystem>().Play();
    }

    public virtual void SetForceToPlayer(Rigidbody playerRB,float force, Vector3 centrePos, float distance)
    {
        PlayerRBOn.Invoke();
        playerRB.AddExplosionForce(force, centrePos, distance);
    }
}
