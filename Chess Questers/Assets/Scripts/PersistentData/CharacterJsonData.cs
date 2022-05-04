using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CharacterJsonData
{
    public int ID;
    public string Name;
    public bool IsFriendly;

    public int Health;
    public int MaxHealth;
    public int MoveActionID;
    //public int[] Actions;
    public List<ActionJsonData> Actions = new List<ActionJsonData>();

    public BattleActionJsonData MoveAction;
    public List<BattleActionJsonData> BattleActions = new List<BattleActionJsonData>();

    public int ActionsPerTurn;
    public int ActionsRemaining;
    
    public int Initiative;

    public int CreatureModelID;

    public int CellX;
    public int CellY;
    public Vector3 Position;

    public int CurrentFacing;

    public CharacterJsonData(string name, int creatureModel, int moveClassID, List<ActionClass> actions, int maxHealth)
    {
        Name = name;
        IsFriendly = true;
        Health = maxHealth;
        MaxHealth = maxHealth;
        ActionsPerTurn = 2;
        ActionsRemaining = 2;

        MoveActionID = moveClassID;
        foreach (var action in actions)
        {
            Actions.Add(new ActionJsonData(action));
        }

        CreatureModelID = creatureModel;
    }


    public CharacterJsonData(string name, int creatureModel, PlayerClass playerClass)
    {
        Name = name;
        IsFriendly = true;
        Health = playerClass.MaxHealth;
        MaxHealth = playerClass.MaxHealth;
        ActionsPerTurn = 2;
        ActionsRemaining = 2;

        //BattleActions.Add(new BattleActionJsonData(playerClass.MoveAction));
        MoveActionID = playerClass.MoveAction.ID;

        foreach (var action in playerClass.AvailableActions)
        {
            BattleActions.Add(new BattleActionJsonData(action));
        }

        CreatureModelID = creatureModel;

    }


    public CharacterJsonData(PlayerCharacter c)
    {
        ID = c.ID;
        Name = c.Name;
        IsFriendly = c.IsFriendly;
        Health = c.Health;
        MaxHealth = c.MaxHealth;
        MoveActionID = c.MoveAction.ID;
        ActionsPerTurn = c.ActionsPerTurn;
        ActionsRemaining = c.ActionsRemaining;

        MoveAction = new BattleActionJsonData(c.MoveAction);
        foreach (var action in c.Actions)
        {
            //Actions.Add(new ActionJsonData(action));
            BattleActions.Add(new BattleActionJsonData(action));
        }
        CreatureModelID = c.CreatureModelID;
        Initiative = c.Initiative;

        CellX = c.X;
        CellY = c.Y;
        Position = c.Position;
        CurrentFacing = c.CurrentFacing;
    }

    public CharacterJsonData(EnemySO enemy)
    {
        ID = enemy.ID;
        Name = enemy.Name;
        IsFriendly = false;
        Health = enemy.Health;
        MaxHealth = enemy.Health;
        MoveActionID = enemy.MoveAction.ID;
        //Actions = enemy.Actions.Select(s => s.ID).ToArray();
        //Actions = enemy.Actions;
        MoveAction = new BattleActionJsonData(enemy.MoveAction);
        foreach (var action in enemy.Actions)
        {
            //Actions.Add(new ActionJsonData(action));
            BattleActions.Add(new BattleActionJsonData(action));
        }
        CreatureModelID = enemy.CreatureModelID;
    }


    public void SetPosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        CellX = cellX;
        CellY = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }

}
