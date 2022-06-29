using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StartState : State
{ 
    public override void Do()
    {
        EnemyBoxer.LookAtTarget();
    }
}
