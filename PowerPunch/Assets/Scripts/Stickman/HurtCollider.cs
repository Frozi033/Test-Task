using BattleVariables;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public class HurtCollider : MonoBehaviour
{
    [SerializeField] private GameObject _currentGameObject;
    [SerializeField] private GameObject _opponentGameObject;
    
    [SerializeField] private MultiAimConstraint _multiAimConstraint;
    
    [SerializeField] private string _opponentFistTag;
    [SerializeField] private float _speed;
    [SerializeField] private bool _hitReactionEnabled;

    private delegate void AnimHitReaction();
    private AnimHitReaction _setAnimWeight;
    private AnimHitReaction _lerpAnimWeight;

    private IDamageable _health;
    private IDamaging _damage;

    void Start()
    {
        _health = _currentGameObject.GetComponent<IDamageable>();
        _damage = _opponentGameObject.GetComponent<IDamaging>();

        if (_hitReactionEnabled)
        {
            _setAnimWeight = SetAnimWeight;
            _lerpAnimWeight = LerpAnimWeight;
        }
    }
    private void FixedUpdate()
    {
        _lerpAnimWeight?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<IDamageable>() != null && other.CompareTag(_opponentFistTag))
        {
            _health.TakeDamage(_damage.damage);
            _setAnimWeight?.Invoke();
        }
    }

    private void SetAnimWeight()
    {
        _multiAimConstraint.weight = Random.Range(0.5f, 1f);
    }

    private void LerpAnimWeight()
    {
        _multiAimConstraint.weight = Mathf.Lerp(_multiAimConstraint.weight, 0, Time.deltaTime * _speed);
    }
}
