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

    [SerializeField] private State _fightState;  // тут я использую паттерн состояние
    [SerializeField] private State _superPowerState;
    [SerializeField] private State _relaxState;
    [SerializeField] private State _startState;
    [SerializeField] private State _waitFightState;
    
    [SerializeField] private List<SuperPower> _superPowerAttacks = new List<SuperPower>();  // тут я использую scriptable object, чтобы задавать поведения при различных суперсилах

    
    private float _ratioLevel = 1;
    
    private bool _relaxing;
    
    private Vector3 _defaultPos;

    
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
        
        StartCoroutine(SelectingSuperAttacks()); // начинаем задержку в применении суперсил
        
        _defaultPos = gameObject.transform.localPosition;
    }


    void Update()
    {
        CurrentState.Do();
    }
    
    public void LookAtTarget()
    {
        Look(_target);  // смотрим на цель
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_relaxing && CurrentSuperPower == null)   // начинаем драться с игроком
        {
            SetState(_fightState);
            FightStart.Invoke();
        }
        else if (other.CompareTag("Player") && (_relaxing || CurrentSuperPower != null))  // только игрок может бить врага, так как у врага есть другие дела
        {
            FightStart.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !_relaxing && CurrentSuperPower == null)  // перестаем драться с игроком
        {
            SetState(_waitFightState);
            FightEnd.Invoke();
        }
        else if (other.CompareTag("Player") && (_relaxing || CurrentSuperPower != null)) // игрок перестает нас бить
        {
            FightEnd.Invoke();
        }
    }

    
    public void SetState(State state)
    {
        CurrentState = Instantiate(state);  // включаем нужное состояние
        CurrentState.EnemyBoxer = this;
        CurrentState.Init(); // инициализируем его
    }
    private void SetSuperPower(SuperPower superPower)
    {
        CurrentSuperPower = Instantiate(superPower);   // включаем суперсилу
        CurrentSuperPower.EnemyBoxer = this;
        
        FightOvering();
        SetState(_superPowerState); // меняем состояние
        
        _myAnimator.SetBool("SuperPower", true);
        
        CurrentSuperPower.Init(_target.gameObject, _playerRoot,_ratioLevel);
    }
    
    private void Damaging()
    {
        CurrentSuperPower.Hitting();
        FightEnd.Invoke();   // этот метод запускается с помощью эвента в анимации
    }
    
    private void SetAnimSpeed(float speed)
    {
        CurrentSuperPower?.Charging(speed);   // этот метод включает и отключает задержку анимации
    }

    public void FinishSuperAttack()
    {
        SetState(_relaxState);

        _relaxing = true;
        
        _myAnimator.SetBool("SuperPower", false);
        _myAnimator.SetBool("RelaxState", true);  // тут мы запускаем состояние и таймер отдыха
        
        StartCoroutine(RelaxingState());
    }

    public void FightStarted()
    {
        Fighting();
        
        _myAnimator.SetBool("Fight", true);  // включаем состояние драки
        
    }
    
    public void FightOvering()
    {
        _myAnimator.SetBool("Fight", false);   // выключаем состояние драки
        
        FightOver();
    }
    
    
    private IEnumerator SelectingSuperAttacks()
    {
        yield return new WaitForSeconds(_timeDelaySuperPower);
        
        if (CurrentSuperPower == null)
        {
            int id = Random.Range(0, _superPowerAttacks.Count);  // тут просто рандомно берем суперсилу и включаем ее
            Debug.Log(id);
            SetSuperPower(_superPowerAttacks[id]);
        }
    }

    private IEnumerator RelaxingState()
    {
        yield return new WaitForSeconds(_timeRelaxState);

        _relaxing = false;

        _myAnimator.SetBool("RelaxState", false);

        if ((_target.position - gameObject.transform.position).magnitude <= 1.5f)
        {
            SetState(_fightState);
        }
        else    // проверяем на каком расстоянии игрок, чтобы сразу же перейти в состояние боя
        {
            SetState(_waitFightState);
        }
        
        StartCoroutine(SelectingSuperAttacks()); // начинаем задержку в применении суперсил 
        
        gameObject.transform.localPosition = _defaultPos; // тут возвращаем игрока в начальную позицию, тк из-за анимации он иногда "съезжал"
    }
}
