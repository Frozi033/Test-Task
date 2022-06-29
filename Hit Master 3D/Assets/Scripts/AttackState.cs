using UnityEngine;

[CreateAssetMenu]
public class AttackState : State
{ 
    public override void Init() {    }

    public override void Do()
    {
        if (IsFinished)
        {
            Debug.Log("Завершили аиминг");
            Player.AttackStateOver();
        }
        Aiming();
    }

    void Aiming()
    {
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.LeftShift))
        {
            Player.Aim();
            Debug.Log("aim");
        }
    }
    
}
