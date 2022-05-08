using JFlex.ChessQuesters.Encounters.Battle.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Chess Questers/Action Result")]
    public class ActionResult : ScriptableObject
    {

        public GridCell Cell;
        public NewBattleAction Action;
        public int X;
        public int Y;

        public List<Creature> Creatures;


        public void Init(GridCell cell, NewBattleAction action)
        {
            Cell = cell;
            X = cell.X;
            Y = cell.Y;
            Action = action;
            Creatures = new List<Creature>();
        }
    }
}