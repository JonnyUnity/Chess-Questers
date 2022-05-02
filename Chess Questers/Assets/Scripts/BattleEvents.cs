using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvents : MonoBehaviour
{

    public static event Action OnSaveGame;

    public static event Action OnBattleStarted;
    public static event Action OnBattleResumed;


    public static event Action OnStartCombat;
    public static event Action OnResumeCombat;

    public static event Action<Creature> OnTurnStart;
    public static event Action OnPlayerStartTurn;
    public static event Action OnPlayerEndTurn;
    public static event Action OnPlayerSelectMove;
    public static event Action<Creature> OnCreatureMoved;
    public static event Action<NewBattleAction> OnPlayerActionSelected;

    public static event Action<NewBattleAction> OnPlayerActionPerformed;

    public static event Action OnTurnOver;

    public static event Action OnAttack;
    public static event Action<Creature, int> OnTakeDamage;
    public static event Action<Creature> OnDeath;

    public static event Action OnBattleVictory;
    public static event Action OnBattleLoss;


    public static event Action<GridCell> OnCellMoveHighlighted;
    public static event Action OnCellMoveUnhighlighted;
    public static event Action<GridCell> OnCellMoveSelected;

    public static event Action<GridCell> OnCellAttackHighlighted;
    public static event Action OnCellAttackUnhighlighted;
    public static event Action<GridCell> OnCellAttackSelected;

    public static event Action<GridCell> OnCellHighlighted;
    

    public static event Action OnCellUnhighlighted;

    public static event Action<GridCell> OnCellSelected;

    public static event Action<int> OnEnemySelectMove;
    public static event Action<int> OnEnemySelectAttack;

    public static event Action<Creature> OnCreatureHover;
    public static event Action<Creature> OnCreatureUnhover;

    public static event Action OnPassTurn;
    public static event Action<NewBattleAction> OnActionStart;
    public static event Action OnActionFinish;



    public static void BattleStarted()
    {
        OnBattleStarted?.Invoke();
    }


    public static void StartCombat()
    {
        OnStartCombat?.Invoke();
    }

    public static void ResumeCombat()
    {
        OnResumeCombat?.Invoke();
    }


    public static void TurnStarted(Creature activeCharacter)
    {
        OnTurnStart?.Invoke(activeCharacter);
    }

    public static void PassedTurn()
    {
        OnPassTurn?.Invoke();
    }

    public static void StartPlayerTurn()
    {
        OnPlayerStartTurn?.Invoke();
    }


    public static void PlayerMoveSelected()
    {
        OnPlayerSelectMove?.Invoke();
    }

    public static void EndPlayerTurn()
    {
        OnPlayerEndTurn?.Invoke();
    }

    public static void TurnOver()
    {
        OnTurnOver?.Invoke();
    }

    public static void HighlightCell(GridCell cell)
    {
        OnCellHighlighted?.Invoke(cell);
    }

    public static void CellMoveHighlighted(GridCell cell)
    {
        OnCellMoveHighlighted?.Invoke(cell);
    }


    public static void CellUnhighlighted()
    {
        OnCellUnhighlighted?.Invoke();
    }

    public static void CellMoveUnhighlighted()
    {
        OnCellMoveUnhighlighted?.Invoke();
    }


    //public static void CellMoveSelected(GridCell cell)
    //{
    //    OnCellMoveSelected?.Invoke(cell);
    //}


    public static void SelectCell(GridCell cell)
    {
        OnCellSelected?.Invoke(cell);
    }


    public static void CreatureMoved(Creature creature)
    {
        OnCreatureMoved?.Invoke(creature);
    }



    public static void CellAttackHighlighted(GridCell cell)
    {
        OnCellAttackHighlighted?.Invoke(cell);
    }

    public static void CellAttackUnhighlighted()
    {
        OnCellAttackUnhighlighted?.Invoke();
    }

    //public static void CellAttackSelected(GridCell cell)
    //{
    //    OnCellAttackSelected?.Invoke(cell);
    //}

    public static void ActionSelected(NewBattleAction action)
    {
        OnPlayerActionSelected?.Invoke(action);
    }


    public static void ActionPerformed(NewBattleAction action)
    {
        OnPlayerActionPerformed?.Invoke(action);
    }


    public static void Victory()
    {
        OnBattleVictory?.Invoke();
    }


    public static void TakeDamage(Creature creature, int damage)
    {
        OnTakeDamage?.Invoke(creature, damage);
    }


    public static void CharacterDied(Creature creature)
    {
        OnDeath?.Invoke(creature);
    }


    public static void Loss()
    {
        OnBattleLoss?.Invoke();
    }


    public static void CreatureHovered(Creature creature)
    {
        OnCreatureHover?.Invoke(creature);
    }


    public static void CreatureUnhovered(Creature creature)
    {
        OnCreatureUnhover?.Invoke(creature);
    }


    public static void ActionStarted(NewBattleAction action)
    {
        OnActionStart?.Invoke(action);
    }

    public static void ActionFinished()
    {
        OnActionFinish?.Invoke();
    }



}
