using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WaitForRunState : State
{
    public State AttackState;
    
    public override void Init()
    {
        var heading = Player.movePoints[Player.index].transform.position - Player.transform.position;
        var distance = heading.magnitude; // расчет дистанции до вейпоинта
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0 && distance <= 0.9)
        {
            Player.SetState(AttackState);
        }
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
        var heading = Player.movePoints[Player.index].transform.position - Player.transform.position;
        var distance = heading.magnitude; // расчет дистанции до вейпоинта
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("sss");
            Player.movePoints[Player.index].SetActive(false);
            Player.agent.SetDestination(Player.movePoints[Player.index].transform.position); // идем к вейпоинту
            Player.animator.SetBool("Run", true);
        }
        if (distance <= 0.9 && Player.animator.GetBool("Run") == true)
        {
            Player.animator.SetBool("Run", false); // выключаем анимацию
            Player.index++;
            IsFinished = true; // состояние завершилось
        }
    }
}
