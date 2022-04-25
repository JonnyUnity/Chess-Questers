using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Creature
{
    public int EnemyID;


    public Brain Brain { get; private set; }

    private EnemyActionResult _enemyAction;

    public void Init(NewEnemyJsonData data, EnemySO enemyObject)
    {
        ID = data.ID;
        EnemyID = data.EnemyID;
        Name = data.Name;
        //_nameText.text = Name;
        IsFriendly = false;
        CreatureModelID = enemyObject.CreatureModelID;

        _portraitSprite = enemyObject.Portrait;

        //_creatureModel = GameManager.Instance.GetCreatureModel(CreatureModelID);
        //_portraitSprite = _creatureModel.Portrait;

        SetInitiative(data.Initiative);
        //MoveClass = GameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveAction = Instantiate(enemyObject.MoveAction);
        MoveClassText = MoveAction.name;

        //Actions = GameManager.Instance.GetActionsWithIDs(data.Actions);
       
        foreach (var action in enemyObject.Actions)
        {
            var jsonData = data.BattleActions.Where(w => w.ID == action.ID).Single();
            var currAction = Instantiate(action);
            currAction.Init(jsonData, enemyObject.Faction);
            Actions.Add(currAction);
        }

        Health = data.Health;
        MaxHealth = data.MaxHealth;
        ActionsPerTurn = data.ActionsPerTurn;

        Brain = enemyObject.Brain;

        CellX = data.CellX;
        CellY = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }



    public void CalcMove()
    {

        GridCell move = Brain.GetMove(this);

        TargetX = move.X;
        TargetY = move.Y;
        TargetPosition = move.Position;

        State = CharacterStatesEnum.MOVING;

    }


    public EnemyActionResult CalcAttack()
    {
        _enemyAction = Brain.GetAction(this);
        return _enemyAction;
    }

}