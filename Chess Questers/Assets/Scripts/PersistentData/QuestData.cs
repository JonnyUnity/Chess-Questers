using System;
using System.Collections.Generic;

public class QuestData
{

    public int MapSeed;
    public int Floor;
    // public Object MapData


    public EncounterTypesEnum CurrentEncounterType;

    public List<ImprovedCharacter> PartyMembers;
    public List<ImprovedCharacter> Enemies;

    public int Battle_ID;
    public int Battle_Layout;
    public NewInitiative Initiative; // will this save to json...


    public QuestData()
    {
        MapSeed = 0;
        Floor = 0;
        CurrentEncounterType = 0;

        PartyMembers = new List<ImprovedCharacter>();

    }


    // convert json data to objects used in game
    public QuestData(QuestJsonData jsonData)
    {
        MapSeed = jsonData.MapSeed;
        Floor = jsonData.Floor;
        CurrentEncounterType = jsonData.CurrentEncounterType;
        
        // characterData => ImprovedCharacter
        PartyMembers = SaveDataManager.DeserializeCharacterData(jsonData.PartyMembers, true);

        Battle_ID = jsonData.Battle_ID;
        Battle_Layout = jsonData.Battle_Layout;
        Initiative = jsonData.Initiative;
        Enemies = SaveDataManager.DeserializeCharacterData(jsonData.Enemies, false);
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