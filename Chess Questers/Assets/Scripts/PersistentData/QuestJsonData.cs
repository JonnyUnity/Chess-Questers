using JFlex.ChessQuesters.Core.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestJsonData
{
    public int MapSeed;
    public int Floor;

    // public Object MapData

    public int EncounterID;
    public EncounterTypesEnum CurrentEncounterType;
    public int TurnNumber;
    public int TurnPointer;

    public CharacterJsonData[] PartyMembers;

    // Battle Encounter data
    public int Battle_ID;
    public int Battle_Layout;
    public EnemyJsonData[] Enemies;


    public QuestJsonData()
    {
        MapSeed = 0;
        Floor = 0;
        CurrentEncounterType = 0;
        TurnNumber = 0;
        TurnPointer = 0;

        PartyMembers = new CharacterJsonData[3];

    }


    public void SetNextEncounter(Encounter encounter)
    {
        Floor++;
        EncounterID = encounter.ID;
        CurrentEncounterType = encounter.Type;
        Battle_ID = encounter.ID;
        Battle_Layout = encounter.Layout;

        EnemyJsonData[] enemies = encounter.GetEnemiesJson();

        Enemies = enemies;

    }

    public void GoToMap()
    {
        CurrentEncounterType = EncounterTypesEnum.Map;
        TurnNumber = 0;
        TurnPointer = 0;
    }


    public bool HasCombatStarted()
    {
        if (TurnNumber > 0)
        {
            return true;
        }
        else if (TurnNumber == 0 && TurnPointer > 0)
        {
            return true;
        }

        return false;

    }


}

public enum EncounterTypesEnum
{
    Map,
    Battle,
    Rest,
    Shop,
    Boss
}
