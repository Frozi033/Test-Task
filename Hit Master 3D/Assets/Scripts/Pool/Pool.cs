using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Pool : MonoBehaviour // реализация пула
{
    [SerializeField] private PoolObject _prefab;
    [SerializeField] private Transform _container;
    [SerializeField] private int _minCapacity;
    [SerializeField] private int _maxCapacity;
    [SerializeField] private bool _autoExpend;

    private List<PoolObject> _pool;

    private void OnValidate()
    {
        if (_autoExpend)
        {
            _maxCapacity = Int32.MaxValue;
        }
    }

    private void Start()
    {
        CreatPool();
    }

    private void CreatPool()
    {
        _pool = new List<PoolObject>(_minCapacity);
        for (int i = 0; i < _minCapacity; i++)
        {
            CreatElement();

        }
    }

    private PoolObject CreatElement(bool isActiveByDefault = false)
    {
        var createdObject = Instantiate(_prefab, _container);
        createdObject.gameObject.SetActive(isActiveByDefault);

        _pool.Add(createdObject);
        return createdObject;
    }

    public bool TryGetElement(out PoolObject element)
    {
        foreach (var item in _pool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                element = item;
                item.gameObject.SetActive(true);
                return true;
            }
        }

        element = null;
        return false;
    }

    public PoolObject GetFreeElement(Vector3 position, Quaternion rotation)
    {
        var element = GetFreeElement(position);
        element.transform.rotation = rotation;
        return element;
    }

    public PoolObject GetFreeElement(Vector3 position)
    {
        var element = GetFreeElement();
        element.transform.position = position;
        return element;
    }

    public PoolObject GetFreeElement()
    {
        if (TryGetElement(out var element))
        {
            return element;
        }

        if (_autoExpend)
        {
            return CreatElement(true);
        }

        if (_pool.Count < _maxCapacity)
        {
            return CreatElement(true);
        }
        
        throw new Exception("Pool is over");
    }
}
