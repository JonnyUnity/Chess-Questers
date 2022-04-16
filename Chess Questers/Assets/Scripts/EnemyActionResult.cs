using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionResult
{

    public GridCell Cell;
    public ActionClass Action;
    public int X;
    public int Y;
    public int Damage;

    public List<Creature> Creatures;

    // Worry about maximising enemy damage, minimising friendly fire later...
    //public List<Creature> EnemyCreatures;
    //public List<Creature> FriendlyCreatures;

    public EnemyActionResult(GridCell cell, ActionClass action)
    {
        Cell = cell;
        Action = action;
        Damage = action.Damage;
        Creatures = new List<Creature>();

    }

}
