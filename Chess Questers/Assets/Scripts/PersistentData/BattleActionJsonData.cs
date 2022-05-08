using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JFlex.ChessQuesters.Core.ScriptableObjects;

[Serializable]
public class BattleActionJsonData
{
    public int ID;

    public int UsesPerTurn;
    public int Cooldown;
    public int ChargesRemaining;
    public bool ActionOnCooldown;


    public BattleActionJsonData(NewBattleAction action)
    {
        ID = action.ID;
        UsesPerTurn = action.UsesPerTurn;
        Cooldown = action.Cooldown;
        ChargesRemaining = action.ChargesRemaining;
        ActionOnCooldown = action.ActionOnCooldown;
    }

}
