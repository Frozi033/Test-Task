using System;
using UnityEngine;

public class RagDollControls : MonoBehaviour
{
    private Animator _animator;
    
    private Rigidbody[] _rigidbodies;
    
    private Collider[] _colliders;

    public bool on;
    private void Start()
    {
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        _colliders = GetComponentsInChildren<Collider>();
        _animator = GetComponent<Animator>();
        
        SetRagdoll(false);
    }

    private void Update()
    {
        if (on)
        {
            SetRagdoll(true);
            on = false;
        }
    }

    private void SetRagdoll(bool enabled)
    {
        _animator.enabled = !enabled;

        foreach (var _rigidbody in _rigidbodies)
        {
            _rigidbody.isKinematic = !enabled;
            _rigidbody.useGravity = enabled;
        }

        foreach (var _collider in _colliders)
        {
            _collider.isTrigger = !enabled;
        }
    }
}
