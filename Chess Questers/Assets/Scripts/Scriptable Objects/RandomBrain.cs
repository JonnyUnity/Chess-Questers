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
}
