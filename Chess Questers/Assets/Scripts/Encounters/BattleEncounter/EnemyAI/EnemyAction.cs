using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction
{
    public ActionClass Action { get; private set; }
    public GridCell TargetCell { get; private set; }

    public EnemyAction(ActionClass action, GridCell cell)
    {
        Action = action;
        TargetCell = cell;
    }

}
