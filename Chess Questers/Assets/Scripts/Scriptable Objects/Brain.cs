using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brain : ScriptableObject
{

    public abstract GridCell GetMove(EnemySenses enemySenses);


}