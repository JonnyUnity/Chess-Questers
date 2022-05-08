using JFlex.ChessQuesters.Core.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Creature
{
    public int EnemyID;


    public Brain Brain { get; private set; }

    private ActionResult _enemyAction;

    public void Init(EnemyJsonData data, EnemySO enemyObject)
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
        ActionsRemaining = data.ActionsRemaining;

        Brain = enemyObject.Brain;

        X = data.CellX;
        Y = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }



    //public void CalcMove()
    //{

    //    GridCell move = Brain.GetMove(this);

    //    TargetX = move.X;
    //    TargetY = move.Y;
    //    TargetPosition = move.Position;

    //    State = CharacterStatesEnum.MOVING;

    //}


    //public EnemyActionResult CalcAttack()
    //{
    //    _enemyAction = Brain.GetAction(this);
    //    return _enemyAction;
    //}

    public void CalcActions()
    {


        StartCoroutine(ActionsCoroutine());



    }

    private IEnumerator ActionsCoroutine()
    {

        Debug.Log("Start enemy turn!");

        ActionsRemaining = ActionsPerTurn;

        while (ActionsRemaining > 0)
        {
            var action = Brain.GetAction(this);

            if (action != null)
            {
                // do action
                Debug.Log("Start enemy action!");

                Debug.Log(action.Action.Name);
                yield return StartCoroutine(DoTurnCoroutine(action));

                Debug.Log("End enemy action!");
            }

            ActionsRemaining--;

        }

        Debug.Log("Finish enemy turn!");

        BattleEvents.TurnOver();

    }


    //public ActionResult CalcAction()
    //{
    //    _enemyAction = Brain.GetAction(this);

    //    if (_enemyAction == null)
    //        return null;

    //    //Debug.Log($"{ActionsRemaining} - {_enemyAction.Action.Name}");

    //    DoAction(_enemyAction);

    //    _enemyAction.Action.DoAction();
    //    ActionsRemaining--;

    //    return _enemyAction;
                
    //}




}