using System;
using UnityEngine;

[CreateAssetMenu]
public class SuperPunch : SuperPower
{
    [SerializeField] private float _radius;
    [SerializeField] private float _angle;
    
    private Rigidbody _rigidBody;
    
    private GameObject _player;
    private GameObject _current;
    
    public static Action<bool, float> UiDangerZone;

    public override void Init(GameObject player, GameObject playerRoot, float levelRatio)
    {
        _player = player;
        _rigidBody = playerRoot.GetComponentInChildren<Rigidbody>();
        
        base.Init(player, playerRoot,levelRatio);
        
        UiDangerZone?.Invoke(true, _angle);
    }

    public override void DO() {    }

    protected override void GameOver(string tag)
    {
        base.GameOver(tag);
        
        UiDangerZone?.Invoke(false, _angle);
    }
    
    bool GetRaycast(Vector3 dir)
    {
        bool result = false;
        
        RaycastHit hit = new RaycastHit();
        Vector3 pos = EnemyBoxer.transform.position + new Vector3(0f,0.7f,0f);
        if (Physics.Raycast(pos, dir, out hit, _radius))
        {
            result = true;
            
            Debug.DrawLine(pos, hit.point, Color.green);
        }
        else
        { 
            Debug.DrawRay(pos, dir * _radius, Color.red);
        }
        
        return result;
    }
	
    bool RayToScan () 
    {
        bool result = false;
        bool a = false;
        bool b = false;
        float j = 0;
        for (int i = 0; i < 4; i++)
        {
            var x = Mathf.Sin(j);
            var y = Mathf.Cos(j);

            j += _angle * Mathf.Deg2Rad / 4;

            Vector3 dir = EnemyBoxer.transform.TransformDirection(new Vector3(x, 0, y));
            if (GetRaycast(dir))
            {
                a = true;
            }

            if(x != 0) 
            {
                dir = EnemyBoxer.transform.TransformDirection(new Vector3(-x, 0, y));
                
                if(GetRaycast(dir)) b = true;
            }
        }

        if (a || b)
        {
            result = true;
        }
        
        return result;
    }
    


    public override void Hitting()
    {
        if (RayToScan())
        {
            SetForceToPlayer(_rigidBody, _interractForce, EnemyBoxer.transform.position, _radius);

            _playerHealth.TakeDamage(_damage);
        }
        
        base.Hitting();

        OverSuperPower();

        UiDangerZone?.Invoke(false, _angle);
    }
}
