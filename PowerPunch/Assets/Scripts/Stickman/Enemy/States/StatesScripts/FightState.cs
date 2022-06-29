using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FightState : State
{
    public override void Init()
    {
       EnemyBoxer.FightStarted();
    }

    public override void Do()
    {
        EnemyBoxer.LookAtTarget();
    }
}
