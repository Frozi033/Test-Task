using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player: StickmanCore
{
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private Transform _target;

    private Vector3[] _transforms;
    private Quaternion[] _rotations;
    
    private delegate void Magneting(Vector3 direction);

    private Magneting _magneting;
    
    void Start()
    {
        Enemy.FightStart += FightStated;
        Enemy.FightEnd += FightOvering;
        SuperPower.PlayerRBOn += SplashHit;
        SuperMagnetPunch.ActivateMagnetingForce += SetMagneting;
    }

    void Update()
    {
        Move();
        LookAtTarget(_target);
    }
    private void Move()
    {
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        
        var tarPos = _target.transform.position;
        var palyerPos = transform.position;
        var dirToTarget = tarPos - palyerPos;
        dirToTarget.y = 0;
        var lookAtTargetRot = Quaternion.LookRotation(dirToTarget);
        var moveVector = lookAtTargetRot * new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);

        MovePlayer(moveVector, _joystick.Vertical, _joystick.Horizontal);
        _magneting?.Invoke(dirToTarget); 
    }
    private void LookAtTarget(Transform target)
    {
        Look(target);
    }

    private void SplashHit()
    {
        if (LifeStatus != Status.Dead)
        {
            RagdollOn();
            RagDollGetUp();
        }
    }

    private void FightStated()
    {
        if (LifeStatus != Status.Dead)
        {
            Fighting();
            _hurtCol.SetActive(true);
            _myAnimator.SetLayerWeight(1, 1);
        }
    }
    private void FightOvering()
    {
        if (LifeStatus != Status.Dead)
        {
            FightOver();
            _hurtCol.SetActive(false);
            _myAnimator.SetLayerWeight(1, 0);
        }
    }

    private void SetMagneting(bool eneble)
    {
        if (eneble)
        {
            _magneting = MoveToTarget;
        }
        else
        {
            _magneting = null;
        }
    }
}
