using System;

[Serializable]
public class BattleEncounterJsonData
{
    public int ID;
    private int _battleLayout;
    private int _battleType;
    private int _biome;
    private int _turnNumber;

    public InitiativeJsonData Initiative;
    public CharacterJsonData[] Enemies;

    public BattleEncounterJsonData(int ID, int battleLayout, CharacterJsonData[] enemies)
    {
        this.ID = ID;
        _battleLayout = battleLayout;
        Enemies = enemies;
    }




}
