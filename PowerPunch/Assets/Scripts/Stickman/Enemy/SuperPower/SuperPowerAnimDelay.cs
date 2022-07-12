using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPowerAnimDelay : MonoBehaviour
{
    [SerializeField] private float _timeDelay;
    

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    public void SetAnimDelay()
    {
        _animator.speed = 0;
        StartCoroutine(ChargingDeley());
    }

    private IEnumerator ChargingDeley()
    {
        yield return new WaitForSeconds(_timeDelay);
        _animator.speed = 1;
    }
}
