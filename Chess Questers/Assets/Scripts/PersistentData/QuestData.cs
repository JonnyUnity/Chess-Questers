using System;
using System.Collections.Generic;

public class QuestData
{

    public int MapSeed;
    public int Floor;
    // public Object MapData


    public EncounterTypesEnum CurrentEncounterType;

    public List<PlayerCharacter> PartyMembers;
    public List<Enemy> Enemies;

    public int Battle_ID;
    public int Battle_Layout;
    public InitiativeData Initiative; // will this save to json...


    public QuestData()
    {
        MapSeed = 0;
        Floor = 0;
        CurrentEncounterType = 0;

        PartyMembers = new List<PlayerCharacter>();

    }

}