using System.Collections;
using UnityEngine;
using BzKovSoft.RagdollTemplate.Scripts.Charachter;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BzRagdoll))]
public class StickmanCore : MonoBehaviour
{
    [SerializeField] protected GameObject _hurtCol;
    
    [SerializeField] protected float _timeDelayGetUp;
    
    
    private CharacterController _myController;
    private Rigidbody _myRigidBody;
    private StickmanCore _mainControl;
    
    private Vector3 _velocity;
    private Vector3 _playerVelocityY;
    
    private float _mySpeed = 5f;
    private float _gravityValue = -10f;

    public float _force;
    
    
    protected Animator _myAnimator; 
    protected BzRagdoll _ragDollController;
    
    
    public bool CanMove { get; private set; }
    public static Status LifeStatus { get; set; }

    public virtual void Awake()
    {
        _ragDollController = GetComponent<BzRagdoll>();
        _myAnimator = GetComponent<Animator>();
        _myController = GetComponent<CharacterController>();
        _mainControl = GetComponent<StickmanCore>();
        
        HPBar.SMBDead += Dead;

        CanMove = true;
        LifeStatus = Status.Live;
    }
    
    protected void MovePlayer(Vector3 direction, float speedV, float speedH)
    {
        _myController.Move(direction * Time.deltaTime * _mySpeed);

        MoveAnimationDirection(speedV, speedH); 
        
        Gravity();
    }

    protected void MoveToTarget(Vector3 direction)
    {
        _myController.Move(direction * Time.deltaTime * _force);
    }
    private void Gravity()
    {
        if (!_myController.isGrounded)
        {
            _playerVelocityY.y += _gravityValue * Time.deltaTime;
        }
        else
        {
            _playerVelocityY.y = 0f;
        }
    }
    protected void Look(Transform target)
    {
        var direction = (target.position - gameObject.transform.position).normalized; 
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        gameObject.transform.rotation = lookRotation;
    }
    
    protected void Fighting()
    {
        //_hurtCol.SetActive(true);
    }
    protected void FightOver()
    {
        // _hurtCol.SetActive(false);
    }

    protected void Dead(string tag)
    {
        if (gameObject.CompareTag(tag))
        {
            RagdollOn();
            //_mainControl.enabled = !enabled;
            LifeStatus = Status.Dead; // включаем состояние смерти
            Debug.Log("Умер");
        }
        _mainControl.enabled = false;
        _hurtCol.SetActive(false);
    }
    
    protected void RagdollOn()
    {
        _ragDollController.RagdollIn();
    }

    protected void RagDollGetUp()
    {
        StartCoroutine(GetUpDelay());
    }
    IEnumerator GetUpDelay()
    {
        yield return new WaitForSeconds(_timeDelayGetUp);
        
        if (LifeStatus != Status.Dead)
        {
            _ragDollController.GetUp();
        }
    }
    
    protected void MoveAnimationDirection(float speedV, float speedH)
    {
        _myAnimator.SetFloat("SpeedV", speedV);
        _myAnimator.SetFloat("SpeedH", speedH);
    }
    
    public enum Status
    {
        Live,
        Dead
    }
}
