using UnityEngine;

[CreateAssetMenu]
public class WaitFightState : State
{
    public override void Init()
    {
        EnemyBoxer.FightOvering();
    }

    public override void Do()
    {
        EnemyBoxer.LookAtTarget();
    }
}
