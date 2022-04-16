using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewEnemyJsonData
{

    public int ID;
    public int EnemyID;
    public string Name;
    public int Health;
    public int MaxHealth;
    public int Initiative;

    public int CellX;
    public int CellY;
    public Vector3 Position;
    public int CurrentFacing;

    public List<ActionJsonData> Actions = new List<ActionJsonData>();


    public NewEnemyJsonData(EnemySO enemy)
    {
        //ID = enemy.ID;
        EnemyID = enemy.ID;
        Name = enemy.Name;
        Health = enemy.Health;
        MaxHealth = enemy.Health;

        foreach (var action in enemy.Actions)
        {
            Actions.Add(new ActionJsonData(action));
        }

    }


    public NewEnemyJsonData(Enemy enemy)
    {
        ID = enemy.ID;
        Name = enemy.Name;
        EnemyID = enemy.EnemyID;

        Health = enemy.Health;
        MaxHealth = enemy.MaxHealth;
        Initiative = enemy.Initiative;
        foreach (var action in enemy.Actions)
        {
            Actions.Add(new ActionJsonData(action));
        }

        CellX = enemy.CellX;
        CellY = enemy.CellY;
        Position = enemy.Position;
        CurrentFacing = enemy.CurrentFacing;
    }


    public void SetPosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        CellX = cellX;
        CellY = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }

}
