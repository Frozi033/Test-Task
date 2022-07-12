using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : StickmanCore
{
    [SerializeField] private Transform _target;
    
    [SerializeField] private GameObject _playerRoot;
    
    [SerializeField] private float _timeDelaySuperPower;
    [SerializeField] private float _timeRelaxState;

    [SerializeField] private State _fightState;
    [SerializeField] private State _superPowerState;
    [SerializeField] private State _relaxState;
    [SerializeField] private State _startState;
    [SerializeField] private State _waitFightState;
    
    [SerializeField] private List<SuperPower> _superPowerAttacks = new List<SuperPower>();
    
    private float _ratioLevel = 1;
    
    private bool _relaxing;
    
    private Vector3 _defaultPos;

    private static readonly int SuperPowerAnim = Animator.StringToHash("SuperPower");
    private static readonly int RelaxStateAnim = Animator.StringToHash("RelaxState");
    private static readonly int FightAnim = Animator.StringToHash("Fight");
    
    public SuperPower CurrentSuperPower;

    public State CurrentState;

    public static Action FightStart;
    public static Action FightEnd;

    private void OnDisable()
    {
        StopAllCoroutines();
        CurrentState = null;
        CurrentSuperPower = null;
    }

    private void Start()
    {
        SetState(_startState);

        StartCoroutine(SelectingSuperAttacks());
        
        _defaultPos = gameObject.transform.localPosition;
    }


    void Update()
    {
        CurrentState.Do();
    }
    
    public void LookAtTarget()
    {
        Look(_target); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_relaxing && CurrentSuperPower == null)  
        {
            SetState(_fightState);
            FightStart.Invoke();
        }
        else if (other.CompareTag("Player") && (_relaxing || CurrentSuperPower != null)) 
        {
            FightStart.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !_relaxing && CurrentSuperPower == null) 
        {
            SetState(_waitFightState);
            FightEnd.Invoke();
        }
        else if (other.CompareTag("Player") && (_relaxing || CurrentSuperPower != null))
        {
            FightEnd.Invoke();
        }
    }

    
    public void SetState(State state)
    {
        CurrentState = Instantiate(state); 
        CurrentState.EnemyBoxer = this;
        CurrentState.Init();
    }
    private void SetSuperPower(SuperPower superPower)
    {
        CurrentSuperPower = Instantiate(superPower);  
        CurrentSuperPower.EnemyBoxer = this;
        
        FightOvering();
        SetState(_superPowerState);
        
        _myAnimator.SetBool(SuperPowerAnim, true);
        
        CurrentSuperPower.Init(_target.gameObject, _playerRoot,_ratioLevel);
    }
    
    private void Damaging()
    {
        CurrentSuperPower.Hitting();
        FightEnd.Invoke(); 
    }
    
    private void SetAnimSpeed(float speed)
    {
        CurrentSuperPower?.Charging(speed); 
    }

    public void FinishSuperAttack()
    {
        SetState(_relaxState);

        _relaxing = true;
        
        _myAnimator.SetBool(SuperPowerAnim, false);
        _myAnimator.SetBool(RelaxStateAnim, true); 
        
        StartCoroutine(RelaxingState());
    }

    public void FightStarted()
    {
        Fighting();
        
        _myAnimator.SetBool(FightAnim, true);
        
    }
    
    public void FightOvering()
    {
        _myAnimator.SetBool(FightAnim, false);
        
        FightOver();
    }
    
    
    private IEnumerator SelectingSuperAttacks()
    {
        yield return new WaitForSeconds(_timeDelaySuperPower);
        
        if (CurrentSuperPower == null)
        {
            int id = Random.Range(0, _superPowerAttacks.Count);
            Debug.Log(id);
            SetSuperPower(_superPowerAttacks[id]);
        }
    }

    private IEnumerator RelaxingState()
    {
        yield return new WaitForSeconds(_timeRelaxState);

        _relaxing = false;

        _myAnimator.SetBool(RelaxStateAnim, false);

        if ((_target.position - gameObject.transform.position).magnitude <= 1.5f)
        {
            SetState(_fightState);
        }
        else    
        {
            SetState(_waitFightState);
        }
        
        StartCoroutine(SelectingSuperAttacks());
        
        gameObject.transform.localPosition = _defaultPos;
    }
}
