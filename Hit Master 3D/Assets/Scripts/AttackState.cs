using System.Collections;
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[RequireComponent(typeof(Pool))]
public class AttackState : State
{
    public State waitForRunState;
    public static bool shooted;
    
    [SerializeField] private Transform _tapPosition;
    

    public override void Init()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Player.SetState(waitForRunState); // есть ли враги?
        }
    }

    public override void Do()
    {
        if (IsFinished)
        {
            return;
        } 
        Aiming();
        
    }

    void Aiming()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
           Ray ray = Camera.main.ScreenPointToRay(touch.position); // отправляем луч от камеры к точке касания
           RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                _tapPosition.transform.position = hit.point;
                var direction = (hit.point - Player.transform.position).normalized; // на плоскости перемещаем в точку касания обьект
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                Player.transform.rotation = lookRotation;
                Player.barrel.forward = new Vector3(direction.x, 0, direction.z);
            }
            Shoot();
            shooted = true;
        }

        if (enemies.Length == 0)
        {
            IsFinished = true;
        }
    }

    void Shoot()
    {
        if (shooted == false)
        {
            Vector3 SpawnPoint = Player.barrel.transform.position; // вычисляем стартовую позицию пули
            Quaternion SpawnRot = Player.barrel.transform.rotation;
        
            PoolObject Fire = Player._pool.GetFreeElement(SpawnPoint, SpawnRot); // берем из пула пулю
        
            Rigidbody BulReg = Fire.GetComponent<Rigidbody>(); // придаем импульс пуле
            BulReg.AddForce(Fire.transform.forward * 10, ForceMode.Impulse);

        }
    }
}
