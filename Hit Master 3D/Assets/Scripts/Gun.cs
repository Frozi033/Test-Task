using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Pool))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _barrel;

    [SerializeField] private Animator _curAnimator;
    [SerializeField] private int _animatorLayer;
    
    [SerializeField] private float _fireRate;
    
    private float _curTimeOut;
    
    private MyAmmo _currentAmmoContr;
    
    private Pool _pool;
    

    private void Start()
    {
        _currentAmmoContr = GetComponent<MyAmmo>();
        _pool = GetComponent<Pool>();
        
        _curAnimator.SetLayerWeight(_animatorLayer, 1);
    }

    private void Update()
    {
        _curTimeOut += Time.deltaTime;
        Debug.DrawRay(_barrel.position, _barrel.forward, Color.red);
    }
    public void PrepareFire()
    {
        if (_curTimeOut > _fireRate && _currentAmmoContr.Ammo >= 0)
        {
            _curTimeOut = 0;

            _curAnimator.Play("Shoot", _animatorLayer);
        }
        else if (_currentAmmoContr.Ammo <= 0)
        {
            
        }
    }

    public void SpawnBullet()
    {
        Vector3 SpawnPoint = _barrel.position; // вычисляем стартовую позицию пули

        Quaternion SpawnRot = _barrel.rotation;

        PoolObject Fire = _pool.GetFreeElement(SpawnPoint, SpawnRot); // берем из пула пулю
        
        Rigidbody BulReg = Fire.GetComponent<Rigidbody>(); // придаем импульс пуле
        BulReg.velocity = Vector3.zero;
        BulReg.AddForce(BulReg.transform.forward * 10f, ForceMode.Impulse);
        
        _currentAmmoContr.AmmoMinus();
    }
    
}