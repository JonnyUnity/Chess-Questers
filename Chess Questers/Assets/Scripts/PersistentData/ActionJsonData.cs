using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionJsonData
{
    public int ID;


    public int ChargesPerTurn;
    public int Cooldown;
    public int Charges;
    public bool ActionOnCooldown;


    public ActionJsonData(ActionClass action)
    {
        ID = action.ID;
        ChargesPerTurn = action.ChargesPerTurn;
        Cooldown = action.Cooldown;
        Charges = action.Charges;
        ActionOnCooldown = action.ActionOnCooldown;
    }


}
