using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Creature
{
    public new string name;

   

    //[SerializeField]
    //public MoveClass moveClass;
    //public int CellX { private set; get; }
    //public int CellY { private set; get; }
    

    public new void Init(string name, int hp, MoveClass moveClass, Sprite sprite, bool isEnemy, GridCell cell)
    {
        // Other setup stuff...
        base.Init(name, hp, moveClass, sprite, isEnemy, cell);
    }

}