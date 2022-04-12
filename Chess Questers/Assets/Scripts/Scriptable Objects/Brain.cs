using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brain : ScriptableObject
{

    public abstract GridCell GetMove(Enemy enemy);

    public abstract EnemyAction GetAction(Enemy enemy);

}
