using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Chess Questers/Battle Action")]
public class NewBattleAction : ScriptableObject
{
    public int ID;
    public bool ForPlayer;
    public bool ForEnemy;

    [Header("UI")]
    public string Name;

    [Multiline]
    public string Description;
    public Sprite Icon;


    [Header("Cooldown Information")]
    public int RefreshAfterXTurns;
    public int Cooldown;

    public int InitialUsesPerTurn;
    public int UsesPerTurn;

    public bool HasCharges;
    public int InitialCharges;
    public int ChargesRemaining;


    public bool ActionOnCooldown;

    [Header("Move Information")]
    public bool IsMove;
    public MoveTypeEnum MoveType;
    public Color DebugColor;
    public int MoveLimit = int.MaxValue;
    public bool IsJumpingMove;
    public string MoveAnimationTrigger;



    [Header("Action Information")]
    [Tooltip("Does this action attack a creature, cast a spell, buff an ally etc")]
    public bool IsAction;

    
    public int BaseDamage;
    public int MinBonusDamage;
    public int MaxBonusDamage;


    [Tooltip("The minimum number of grid cells away that the action can be performed")]
    public int MinRange;
    [Tooltip("The maximum number of grid cells away that the action can be performed")]
    public int MaxRange;

    public bool IsRanged;
    public bool IsAttack;
    public bool IncludeDiagonals;
    public string ActionAnimationTrigger;

    public GameObject ActionTemplatePrefab;
    public Vector2[] AdditionalAttackedCellOffsets;
    public CreatureRuntimeSet Friendlies { get; private set; }
    public CreatureRuntimeSet NonFriendlies { get; private set; }


    public string DamageRangeText
    {
        get
        {
            return $"{BaseDamage + MinBonusDamage} to {BaseDamage + MaxBonusDamage} Damage";
        }
    }


    public int Damage
    { 
        get
        {
            return BaseDamage + Random.Range(MinBonusDamage, MaxBonusDamage);
        }
    }

    public bool IsActive
    {
        get
        {
            if (UsesPerTurn == 0)
            {
                return false;
            }
            else if (ActionOnCooldown)
            {
                return false;
            }
            else if (HasCharges && ChargesRemaining == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public CreatureRuntimeSet TargetCreatures
    {
        get
        {
            return IsAttack ? NonFriendlies : Friendlies;
        }
    }


    #region Functions

    public void Init(BattleActionJsonData data, Faction faction)
    {
        UsesPerTurn = data.UsesPerTurn;
        Cooldown = data.Cooldown;
        ChargesRemaining = data.ChargesRemaining;
        ActionOnCooldown = data.ActionOnCooldown;

        Friendlies = faction.Friendlies;
        NonFriendlies = faction.NonFriendlies;

    }


    public void StartOfBattle()
    {
        UsesPerTurn = InitialUsesPerTurn;
        ChargesRemaining = InitialCharges;
        Cooldown = RefreshAfterXTurns;
        ActionOnCooldown = false;
    }


    public void DoAction()
    {
        UsesPerTurn--;
        if (HasCharges)
        {
            ChargesRemaining--;
        }
        ActionOnCooldown = true;
    }


    public void EndOfTurn()
    {
        UsesPerTurn = InitialUsesPerTurn;
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


    #endregion

}
