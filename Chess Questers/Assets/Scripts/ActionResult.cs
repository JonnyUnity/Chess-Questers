using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Questers/Action Result")]
public class ActionResult : ScriptableObject
{

    public GridCell Cell;
    public NewBattleAction Action;
    public int X;
    public int Y;

    public List<Creature> Creatures;

    // Worry about maximising enemy damage, minimising friendly fire later...
    //public List<Creature> EnemyCreatures;
    //public List<Creature> FriendlyCreatures;

    //public ActionResult(GridCell cell, NewBattleAction action)
    //{
    //    Cell = cell;
    //    X = cell.X;
    //    Y = cell.Y;
    //    Action = action;
    //    Damage = action.Damage;
    //    Creatures = new List<Creature>();

    //}

    public void Init(GridCell cell, NewBattleAction action)
    {
        Cell = cell;
        X = cell.X;
        Y = cell.Y;
        Action = action;
        Creatures = new List<Creature>();
    }


}
