using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Chess Questers/Action Class")]
public class ActionClass : ScriptableObject
{
    public int ID;
    public string Name;

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

    public int InitialCharges;
    public int Charges;

    public Vector2[] additionalAttackedCellOffsets;


    public void StartOfBattle()
    {
        ChargesPerTurn = InitialChargesPerTurn;
        Charges = InitialCharges;
        Cooldown = RefreshAfterXTurns;
    }

    public void DoAction()
    {
        ChargesPerTurn--;
        Charges--;
    }


    public void EndOfTurn()
    {
        ChargesPerTurn = InitialChargesPerTurn;
        Cooldown--;
        if (Cooldown == 0)
        {
            Cooldown = RefreshAfterXTurns;
        }

    }



    public bool IsActive()
    {
        if (ChargesPerTurn == 0)
        {
            return false;
        }
        else if (Cooldown > 0)
        {
            return false;
        }
        else if (Charges == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
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

