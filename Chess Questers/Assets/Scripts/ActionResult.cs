using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Questers/Action Result")]
public class ActionResult : ScriptableObject
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

    public ActionResult(GridCell cell, ActionClass action)
    {
        Cell = cell;
        Action = action;
        Damage = action.Damage;
        Creatures = new List<Creature>();

    }

}
