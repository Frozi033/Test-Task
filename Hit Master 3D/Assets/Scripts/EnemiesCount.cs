using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemiesCount : MonoBehaviour
{
    [SerializeField] private List<Stage> _stage = new List<Stage>();

    private int _enemiesLeft;
    private int id;

    private void Start()
    {
        id = 0;
        Bullet.EnemyHit += EnemyMinus;
    }

    private void NextStage()
    {
        if (id == _stage.Count - 1)
        {
             SceneManager.LoadScene(0);
        }
        else
        {
            _enemiesLeft = _stage[id++].Count;
        }
    }

    private void EnemyMinus()
    {
        _enemiesLeft--;
        if (_enemiesLeft <= 0)
        {
            NextStage();
        }
    }
}
