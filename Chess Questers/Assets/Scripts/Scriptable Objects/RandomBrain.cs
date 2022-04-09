using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Questers/Brains/Random")]
public class RandomBrain : Brain
{
    public override GridCell GetMove(EnemySenses enemySenses)
    {
        throw new NotImplementedException();

        //var grid = enemySenses.Grid;

        //List<GridCell> cells = grid.GetMoves()

    }
}
