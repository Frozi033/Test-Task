using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [SerializeField] private Camera _myCam;

    [SerializeField] private GameObject _tapPosWorld;
    [SerializeField] private List<GameObject> movePoints = new List<GameObject>();

    [SerializeField] private Gun _currentGun;

    [SerializeField] private State RunState;
    [SerializeField] private State AttackState;
    
    private Animator _myAnimator;
    
    private NavMeshAgent _myNavMeshAgent;

    private Rigidbody _rb;
    
    private int _index;

    private float _speed;
    
    private delegate void Arrived();

    private Arrived OnWay;

    [Header("Actual State")]
    public State CurrentState;

    
    private void Start()
    {
        SetState(RunState);
        
        _myAnimator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _myNavMeshAgent = GetComponent<NavMeshAgent>();
        
        _index = 0;
    }

    private void Update()
    {
        CurrentState.Do();
    }

    public void RunToPoint()
    {
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            
            movePoints[_index].SetActive(false);

            _myNavMeshAgent.SetDestination(movePoints[_index].transform.position); // идем к вейпоинту
            OnWay = DistanceToDestination;
        }

        _speed = (_myNavMeshAgent.destination - gameObject.transform.position).magnitude;
        
        SetAnimSpeed(_speed);
         OnWay?.Invoke();
    }

    private void DistanceToDestination()
    {
        if ((_myNavMeshAgent.destination - gameObject.transform.position).magnitude == 0)
        {
            // _speed = 0;
            //SetAnimSpeed(_speed);
            
            SetState(AttackState);
            
            _index++;

            OnWay = null;
        }
    }

    public void Aim()
    {
        var touch = Input.GetTouch(0);

        Ray ray = _myCam.ScreenPointToRay(touch.position); // отправляем луч от камеры к точке касания
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            _tapPosWorld.transform.position = hit.point; // на плоскости перемещаем в точку касания обьект
            
            var direction = (hit.point - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
            
            _currentGun.PrepareFire();
        }
    }

    public void Fire()
    {
        _currentGun.SpawnBullet();
    }

    public void AttackStateOver()
    {
        SetState(RunState);
    }

    private void SetAnimSpeed(float speed)
    {
        _myAnimator.SetFloat("Speed", speed);
    }

    public void SetState(State state)
    {
        CurrentState = Instantiate(state); // паттерн "Состояние"
        CurrentState.Player = this;
        CurrentState.Init();
    }
}

