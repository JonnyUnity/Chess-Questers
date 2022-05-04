using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Chess Questers/Brains/Random")]
public class RandomBrain : Brain
{

    //public override GridCell GetMove(Enemy me)
    //{
    //    //throw new NotImplementedException();

    //    //GameGrid grid = enemySenses.Grid;
    //    //Enemy thisEnemy = enemySenses.Enemy;

    //    List<GridCell> cells = GameGrid.Instance.GetMoves(me.MoveAction, me.CellX, me.CellY);
    //    //List<Vector2> cells = GameGrid.Instance.GetMovesNew(me.MoveClass, me.CellX, me.CellY);


    //    //Vector2 chosenCoord = cells[Random.Range(0, cells.Count)];

    //    //GridCell chosenMove = GameGrid.Instance.GetCell((int)chosenCoord.x, (int)chosenCoord.y);

    //    //List<GridCell> cells = grid.GetMoves()

    //    GridCell chosenMove = cells[Random.Range(0, cells.Count)];

    //    return chosenMove;
    //}

    //public override EnemyActionResult GetAction(Enemy enemy)
    //{

    //    List<EnemyActionResult> results = new List<EnemyActionResult>();

    //    foreach (var action in enemy.Actions.Where(w => w.IsActive).ToList())
    //    {
    //        List<EnemyActionResult> actionResults = GameGrid.Instance.GetTargetsOfActionNew(action, enemy.CellX, enemy.CellY);
    //        results.AddRange(actionResults);

    //    }

    //    if (results.Count > 0)
    //    {
    //        return results[Random.Range(0, results.Count)];
    //    }

    //    return null;  

    //}


    public override ActionResult GetAction(Enemy enemy)
    {
        List<ActionResult> results = new List<ActionResult>();
        if (enemy.MoveAction.IsActive)
        {
            List<ActionResult> moveResults = GameGrid.Instance.GetEnemyMoves(enemy.MoveAction, enemy.X, enemy.Y);
            results.AddRange(moveResults);
        }

        foreach (var action in enemy.Actions.Where(w => w.IsActive).ToList())
        {
            List<ActionResult> actionResults = GameGrid.Instance.GetTargetsOfActionNew(action, enemy.X, enemy.Y);
            results.AddRange(actionResults);

        }

        if (results.Count > 0)
        {
            return results[Random.Range(0, results.Count)];
        }

        return null;

    }

}
