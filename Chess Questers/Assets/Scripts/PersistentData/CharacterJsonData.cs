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
    public int MoveClassID;
    //public int[] Actions;
    public List<ActionJsonData> Actions = new List<ActionJsonData>();
    public int Initiative;

    public int CharacterModel;

    public int CellX;
    public int CellY;
    public Vector3 Position;

    public int CurrentFacing;

    public CharacterJsonData(string name, int characterModel, int moveClassID, List<ActionClass> actions, int maxHealth)
    {
        Name = name;
        IsFriendly = true;
        Health = maxHealth;
        MaxHealth = maxHealth;
        MoveClassID = moveClassID;
        foreach (var action in actions)
        {
            Actions.Add(new ActionJsonData(action));
        }

        CharacterModel = characterModel;
    }

    public CharacterJsonData(PlayerCharacter c)
    {
        ID = c.ID;
        Name = c.Name;
        IsFriendly = c.IsFriendly;
        Health = c.Health;
        MaxHealth = c.MaxHealth;
        MoveClassID = c.MoveClass.ID;
        foreach (var action in c.Actions)
        {
            Actions.Add(new ActionJsonData(action));
        }
        CharacterModel = c.CharacterModel;
        Initiative = c.Initiative;

        CellX = c.CellX;
        CellY = c.CellY;
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
        MoveClassID = enemy.MoveClass.ID;
        //Actions = enemy.Actions.Select(s => s.ID).ToArray();
        //Actions = enemy.Actions;
        foreach (var action in enemy.Actions)
        {
            Actions.Add(new ActionJsonData(action));
        }
        CharacterModel = enemy.CharacterModel;
    }


    public void SetPosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        CellX = cellX;
        CellY = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }

}
