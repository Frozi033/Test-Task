using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Pool))]
public class Player : MonoBehaviour
{
    
    private float _shootPauseTime = 5f;

    public State WaitForRunState;
    public State AttackState;
    public Animator animator;
    public NavMeshAgent agent;
    public List<GameObject> movePoints = new List<GameObject>();
    public int index;
    public Pool _pool; 
    public Transform barrel;

    [Header("Actual State")]
    public State CurrentState;

    private void Update()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (!CurrentState.IsFinished)
        {
            CurrentState.Do();
        }
        else
        {
            if (enemies.Length == 0)
            {
                SetState(WaitForRunState);
            }
            else if (enemies.Length > 0)
            {
                SetState(AttackState);
            }
        }
       if (global::AttackState.shooted == true)
        {
            StartCoroutine(ShootPause());
        }
    }

    private void Start()
    {
        SetState(WaitForRunState);
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _pool = GetComponent<Pool>();
        index = 0;
    }

    public void SetState(State state)
    {
        CurrentState = Instantiate(state); // паттерн "Состояние"
        CurrentState.Player = this;
        CurrentState.Init();
    }
    private IEnumerator ShootPause()
    {
        yield return new WaitForSeconds(_shootPauseTime); // таймер для задержки между выстрелами
        global::AttackState.shooted = false;
    }
}
