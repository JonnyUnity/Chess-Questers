using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyJsonData : CharacterJsonData
{
    public int EnemyID;


    public EnemyJsonData(string name, int enemyID, int characterModel, int moveClassID, int[] actions, int maxHealth) : base(name, characterModel, moveClassID, actions, maxHealth)
    {
        EnemyID = enemyID;
    }

    public EnemyJsonData(Enemy enemy) : base(enemy)
    {
        //ID = enemy.ID;
        //Name = enemy.Name;
        //IsFriendly = false;
        //Health = enemy.Health;
        //MaxHealth = enemy.Health;
        //MoveClassID = enemy.MoveClass.ID;
        //Actions = enemy.Actions.Select(s => s.ID).ToArray();
        //CharacterModel = enemy.CharacterModel;
        EnemyID = enemy.ID;
    }

    public EnemyJsonData(ImprovedCharacter c) : base(c)
    {
        EnemyID = c.EnemyID;
        //ID = c.ID;
        //Name = c.Name;
        //IsFriendly = c.IsFriendly;
        //Health = c.Health;
        //MaxHealth = c.MaxHealth;
        //MoveClassID = c.MoveClass.ID;
        //Actions = c.Actions.Select(s => s.ID).ToArray();
        //CharacterModel = c.CharacterModel;

        //CellX = c.CellX;
        //CellY = c.CellY;
        //Position = c.Position;
        //CurrentFacing = c.CurrentFacing;
    }



}
