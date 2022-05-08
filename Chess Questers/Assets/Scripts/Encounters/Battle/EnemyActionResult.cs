using JFlex.ChessQuesters.Core.ScriptableObjects;
using JFlex.ChessQuesters.Encounters.Battle.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionResult
{

    public GridCell Cell;
    public NewBattleAction Action;
    public int X;
    public int Y;
    public int Damage;

    public List<Creature> Creatures;


    public EnemyActionResult(GridCell cell, NewBattleAction action)
    {
        Cell = cell;
        X = cell.X;
        Y = cell.Y;
        Action = action;
        Damage = action.BaseDamage;
        Creatures = new List<Creature>();

    }

}
