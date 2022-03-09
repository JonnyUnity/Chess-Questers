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

    public EncounterTypesEnum CurrentEncounterType;

    public CharacterJsonData[] PartyMembers;

    // Battle Encounter data
    public int Battle_ID;
    public int Battle_Layout;
    public CharacterJsonData[] Enemies;
    public InitiativeData Initiative;


    public QuestJsonData()
    {
        MapSeed = 0;
        Floor = 0;
        CurrentEncounterType = 0;

        PartyMembers = new CharacterJsonData[3];

    }

    // create json version to save
    public QuestJsonData(QuestData data)
    {
        // ImprovedCharacter => characterData
        MapSeed = data.MapSeed;
        Floor = data.Floor;
        CurrentEncounterType = data.CurrentEncounterType;

        PartyMembers = SaveDataManager.SerializeCharacterData(data.PartyMembers);

        Battle_ID = data.Battle_ID;
        Battle_Layout = data.Battle_Layout;
        Enemies = SaveDataManager.SerializeCharacterData(data.Enemies);
        Initiative = data.Initiative;

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
