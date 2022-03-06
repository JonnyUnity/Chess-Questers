using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class CharacterJsonData
{
    public int ID { get; private set; }
    public string Name;
    public bool IsFriendly;

    public int Health;
    public int MaxHealth;
    public int MoveClassID;
    public int[] Actions;

    public int CharacterModel;

    public int CellX;
    public int CellY;
    public Vector3 Position;

    public int CurrentFacing;

    public CharacterJsonData(ImprovedCharacter c)
    {
        ID = c.ID;
        Name = c.Name;
        IsFriendly = c.IsFriendly;
        Health = c.Health;
        MaxHealth = c.MaxHealth;
        MoveClassID = c.MoveClass.ID;
        Actions = c.Actions.Select(s => s.ID).ToArray();
        CharacterModel = c.CharacterModel;

        CellX = c.CellX;
        CellY = c.CellY;
        Position = c.Position;
        CurrentFacing = c.CurrentFacing;
    }

    public CharacterJsonData(Enemy enemy)
    {
        ID = enemy.ID;
        Name = enemy.Name;
        IsFriendly = false;
        Health = enemy.Health;
        MaxHealth = enemy.Health;
        MoveClassID = enemy.MoveClass.ID;
        Actions = enemy.Actions.Select(s => s.ID).ToArray();
        CharacterModel = enemy.CharacterModel;
    }

}
