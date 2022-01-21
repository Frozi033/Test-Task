
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Pool))]
public class Bullet : MonoBehaviour
{
    private PoolObject _poolObject;

    [SerializeField] private float _timeToLife;
    

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
            Destroy(other.gameObject); // тут лучше реализовать пул врагов, но у нас их достаточно мало, поэтому я не стал тратить на это время
            _poolObject.ReturnToPool();
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
