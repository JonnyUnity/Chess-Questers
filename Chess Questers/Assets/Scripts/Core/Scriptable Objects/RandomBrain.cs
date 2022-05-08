using JFlex.ChessQuesters.Encounters.Battle.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Chess Questers/Brains/Random")]
    public class RandomBrain : Brain
    {
        public override ActionResult GetAction(Enemy enemy)
        {
            List<ActionResult> results = new List<ActionResult>();

            // Collect all possible actions and then pick one at random
            if (enemy.MoveAction.IsActive)
            {
                List<ActionResult> moveResults = GameGrid.Instance.GetEnemyMoves(enemy.MoveAction, enemy.X, enemy.Y);
                results.AddRange(moveResults);
            }

            foreach (var action in enemy.Actions.Where(w => w.IsActive).ToList())
            {
                List<ActionResult> actionResults = GameGrid.Instance.GetActionResults(action, enemy.X, enemy.Y);
                results.AddRange(actionResults);
            }

            if (results.Count > 0)
            {
                return results[Random.Range(0, results.Count)];
            }

            return null;
        }
    }
}