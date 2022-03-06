using System;
using System.Linq;
using UnityEngine;


public class ImprovedCharacter
{
    public int ID;
    public string Name { get; private set; }
    public bool IsFriendly { get; private set; }

    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    // Move Class properties
    public MoveClass MoveClass;
    public string MoveClassText { get; private set; }

    public Color MoveClassColor => MoveClass.DebugColor;

    // Action Class properties
    public AttackClass[] Actions;
    public string ActionsText => string.Join(",", Actions.Select(s => s.Name).ToArray());


    public int CharacterModel { get; private set; }

        
    
    public Vector3 Position;
    public int CellX;
    public int CellY;
    public int CurrentFacing;

    
    public ImprovedCharacter(string name, int characterModel, MoveClass moveClass, AttackClass[] attacks, int maxHealth)
    {
        Name = name;
        IsFriendly = true;

        CharacterModel = characterModel;
        MoveClass = moveClass;
        MoveClassText = moveClass.name;

        Actions = attacks;

        Health = maxHealth;
        MaxHealth = maxHealth;
    }

    public ImprovedCharacter(CharacterJsonData data, bool isFriendly)
    {
        ID = data.ID;
        Name = data.Name;
        IsFriendly = data.IsFriendly;
        CharacterModel = data.CharacterModel;
        MoveClass = NewGameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveClassText = MoveClass.name;

        Actions = NewGameManager.Instance.GetActionsWithIDs(data.Actions);
        Health = data.Health;
        MaxHealth = data.MaxHealth;

    }

    public ImprovedCharacter(Enemy data)
    {
        ID = data.ID;
        Name = data.Name;
        IsFriendly = false;
        CharacterModel = data.CharacterModel;
        MoveClass = data.MoveClass;
        MoveClassText = MoveClass.name;

        Actions = data.Actions;
        Health = data.Health;
        MaxHealth = data.Health;
    }


    public void UpdateHealth(int currentHealth)
    {
        Health = currentHealth;
    }


    public void UpdatePosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        CellX = cellX;
        CellY = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }




}
