using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{

    public static event Action OnSaveGame;
    
    public static event Action OnBattleStarted;
    public static event Action OnBattleResumed;
    

    public static event Action<List<ImprovedCharacter>> OnRollInitiative;

    public static event Action<int> OnTurnStart;
    public static event Action OnPlayerStartTurn;
    public static event Action OnPlayerSelectMove;
    public static event Action OnTurnOver;

    public static event Action OnTakeDamage;
    public static event Action OnDeath;

    public static event Action OnBattleVictory;
    public static event Action OnBattleLoss;



    public static void BattleStarted()
    {
        OnBattleStarted?.Invoke();
    }


    public static void RollInitiative(List<ImprovedCharacter> characters)
    {
        OnRollInitiative?.Invoke(characters);
    }


    public static void TurnStarted(int characterID)
    {
        OnTurnStart?.Invoke(characterID);
    }


    public static void Victory()
    {
        OnBattleVictory?.Invoke();
    }

    public static void Loss()
    {
        OnBattleLoss?.Invoke();
    }


    



}
