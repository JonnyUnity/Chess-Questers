using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Chess Questers/Brains/Random")]
public class RandomBrain : Brain
{

    public override GridCell GetMove(Enemy me)
    {
        //throw new NotImplementedException();

        //GameGrid grid = enemySenses.Grid;
        //Enemy thisEnemy = enemySenses.Enemy;

        List<GridCell> cells = GameGrid.Instance.GetMoves(me.MoveClass, me.CellX, me.CellY);


        GridCell chosenMove = cells[Random.Range(0, cells.Count)];

        //List<GridCell> cells = grid.GetMoves()

        return chosenMove;
    }

    public override ActionResult GetAction(Enemy enemy)
    {

        List<ActionResult> results = new List<ActionResult>();

        CreatureRuntimeSet targetCreatures = enemy.Faction.GetTargetFaction(false);

        foreach (var action in enemy.Actions)
        {
            List<ActionResult> actionResults = GameGrid.Instance.GetTargetsOfActionNew(action, targetCreatures, enemy.CellX, enemy.CellY);
            results.AddRange(actionResults);

        }

        // choose random attack
        //var randomAction = enemy.Actions[Random.Range(0, enemy.Actions.Length)];

        // check for null action?
        

        // choose random target
        //List<GridCell> targetCells = GameGrid.Instance.GetTargetsOfAction(randomAction, enemy.CellX, enemy.CellY);

        //List<ActionResult> results = GameGrid.Instance.GetTargetsOfActionNew(randomAction, enemy.Faction.GetTargetFaction(false), enemy.CellX, enemy.CellY);

        
        //GridCell target = null;


        if (results.Count > 0)
        {
            return results[Random.Range(0, results.Count)];
        }

        return null;

        //if (targetCells.Count > 0)
        //{
        //    target = targetCells[Random.Range(0, targetCells.Count)];
        //}




        //return new EnemyAction(randomAction, target);       

    }

}
