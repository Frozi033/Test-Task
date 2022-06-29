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
        EnemyBoxer.CurrentSuperPower.DO(); // передаем update в саму супер силу
    }

    private void Finish()
    {
        EnemyBoxer.FinishSuperAttack(); // завершаем супер атаку
        SuperPower.SuperAttackOver -= Finish;
    }
}
