using UnityEngine;

[CreateAssetMenu]
public class RunState : State
{
    public override void Init()
    {
    }

    public override void Do()
    {
        if (IsFinished)
        {
            return;
        }
        Run();
    }

    void Run()
    {
        Player.RunToPoint();
    }
}
