using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    [HideInInspector] public Enemy EnemyBoxer;
    
    
    public virtual void Init() { }

    public abstract void Do();
    
}
