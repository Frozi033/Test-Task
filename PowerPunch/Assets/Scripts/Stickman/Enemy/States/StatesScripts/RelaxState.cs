using UnityEngine;

[CreateAssetMenu]
public class RelaxState : State
{
    public override void Init()
    {
        EnemyBoxer.CurrentSuperPower = null;
    }

    public override void Do()
    {
    }
}
