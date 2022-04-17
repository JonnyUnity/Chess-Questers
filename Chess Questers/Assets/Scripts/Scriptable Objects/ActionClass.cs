using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Chess Questers/Action Class")]
[Serializable]
public class ActionClass : ScriptableObject
{
    public int ID;
    public string Name;

    [Multiline]
    public string Description;
    public Sprite Icon;
    
    public int Damage;
    public int MinRange;
    public int MaxRange;

    public GameObject AttackTemplatePrefab;

    public ActionShapesEnum Shape;



    public bool IsRanged;
    public bool IsAttack;

    public bool MoveTarget;

    public bool ForPlayer;
    public bool ForEnemy;

    public int RefreshAfterXTurns;
    public int Cooldown;

    public int InitialChargesPerTurn;
    public int ChargesPerTurn;

    public bool HasCharges;
    public int InitialCharges;
    public int Charges;

    public Vector2[] additionalAttackedCellOffsets;

    public bool ActionOnCooldown;

    public void StartOfBattle()
    {
        ChargesPerTurn = InitialChargesPerTurn;
        Charges = InitialCharges;
        Cooldown = RefreshAfterXTurns;
        ActionOnCooldown = false;
    }

    public void DoAction()
    {
        ChargesPerTurn--;
        if (HasCharges)
        {
            Charges--;
        }        
        ActionOnCooldown = true;
    }


    public void EndOfTurn()
    {
        ChargesPerTurn = InitialChargesPerTurn;
        if (ActionOnCooldown)
        {
            Cooldown--;
            if (Cooldown == 0)
            {
                Cooldown = RefreshAfterXTurns;
                ActionOnCooldown = false;
            }
        }
    }



    public bool IsActive()
    {
        if (ChargesPerTurn == 0)
        {
            return false;
        }
        else if (ActionOnCooldown)
        {
            return false;
        }
        else if (HasCharges && Charges == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Init(ActionJsonData data)
    {
        ChargesPerTurn = data.ChargesPerTurn;
        Cooldown = data.Cooldown;
        Charges = data.Charges;
        ActionOnCooldown = data.ActionOnCooldown;
    }




}

public enum ActionTypesEnum
{
    ADJACENT,
    LINE,
    CIRCLE,

}

public enum ActionShapesEnum
{
    POINT,
    LINE,
    CIRCLE
}

