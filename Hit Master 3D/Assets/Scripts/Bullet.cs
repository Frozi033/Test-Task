using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class Bullet : MonoBehaviour
{
    private PoolObject _poolObject;

    [SerializeField] private float _timeToLife;
    
    public static Action EnemyHit;
    

    private void Start()
    {
        _poolObject = GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        StartCoroutine(Destroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Enemy"))
        {
            _poolObject.ReturnToPool();
            
            EnemyHit.Invoke();
        }
        else
        {
            _poolObject.ReturnToPool();
        }
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(_timeToLife); // куротина по возвращению пуль в пул по истечению времени
        _poolObject.ReturnToPool();
    }
}
