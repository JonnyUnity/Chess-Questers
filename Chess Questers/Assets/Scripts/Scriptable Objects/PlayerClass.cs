using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Class", menuName = "Chess Questers/Player Class")]
public class PlayerClass : ScriptableObject
{
    public int ID;
    public string Name;

    [Multiline]
    public string Description;

    public int MaxHealth;

    [Header("Actions")]
    public NewBattleAction MoveAction;
    public NewBattleAction[] AvailableActions;

}
