using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvents : MonoBehaviour
{

    public static event Action OnSaveGame;

    public static event Action OnBattleStarted;
    public static event Action OnBattleResumed;


    public static event Action<List<Creature>> OnRollInitiative;

    public static event Action<int> OnTurnStart;
    public static event Action OnPlayerStartTurn;
    public static event Action OnPlayerSelectMove;
    public static event Action OnPlayerSetupActions;
    public static event Action<int,ActionClass,int,int> OnPlayerActionSelected;
    public static event Action OnTurnOver;

    public static event Action OnAttack;
    public static event Action<int, int> OnTakeDamage;
    public static event Action<int, bool> OnDeath;

    public static event Action OnBattleVictory;
    public static event Action OnBattleLoss;


    public static event Action<GridCell> OnCellMoveHighlighted;
    public static event Action OnCellMoveUnhighlighted;
    public static event Action<GridCell> OnCellMoveSelected;

    public static event Action<GridCell> OnCellAttackHighlighted;
    public static event Action OnCellAttackUnhighlighted;
    public static event Action<GridCell> OnCellAttackSelected;

    public static event Action<int> OnEnemySelectMove;
    public static event Action<int> OnEnemySelectAttack;

    public static void BattleStarted()
    {
        OnBattleStarted?.Invoke();
    }


    public static void RollInitiative(List<Creature> characters)
    {
        OnRollInitiative?.Invoke(characters);
    }


    public static void TurnStarted(int characterID)
    {
        OnTurnStart?.Invoke(characterID);
    }

    public static void TurnOver()
    {
        OnTurnOver?.Invoke();
    }

    public static void CellMoveHighlighted(GridCell cell)
    {
        OnCellMoveHighlighted?.Invoke(cell);
    }

    public static void CellMoveUnhighlighted()
    {
        OnCellMoveUnhighlighted?.Invoke();
    }


    public static void CellMoveSelected(GridCell cell)
    {
        OnCellMoveSelected?.Invoke(cell);
    }


    public static void CharacterFinishedMoving()
    {
        OnPlayerSetupActions?.Invoke();
    }



    public static void CellAttackHighlighted(GridCell cell)
    {
        OnCellAttackHighlighted?.Invoke(cell);
    }

    public static void CellAttackUnhighlighted()
    {
        OnCellAttackUnhighlighted?.Invoke();
    }

    public static void CellAttackSelected(GridCell cell)
    {
        OnCellAttackSelected?.Invoke(cell);
    }

    public static void ActionSelected(int characterID, ActionClass action, int x, int y)
    {
        OnPlayerActionSelected?.Invoke(characterID, action, x, y);
    }


    public static void Victory()
    {
        OnBattleVictory?.Invoke();
    }


    public static void TakeDamage(int characterID, int damage)
    {
        OnTakeDamage?.Invoke(characterID, damage);
    }


    public static void CharacterDied(int characterID, bool isFriendly)
    {
        OnDeath?.Invoke(characterID, isFriendly);
    }


    public static void Loss()
    {
        OnBattleLoss?.Invoke();
    }


    



}
