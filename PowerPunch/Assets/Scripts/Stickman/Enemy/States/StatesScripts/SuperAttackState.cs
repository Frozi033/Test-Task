using UnityEngine;

[CreateAssetMenu]
public class SuperAttackState : State
{
    public override void Init()
    {
        SuperPower.SuperAttackOver += Finish;
    }
    
    public override void Do()
    {
        EnemyBoxer.CurrentSuperPower.DO();
    }

    private void Finish()
    {
        EnemyBoxer.FinishSuperAttack();
        SuperPower.SuperAttackOver -= Finish;
    }
}
