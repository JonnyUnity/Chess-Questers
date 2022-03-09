using System;

[Serializable]
public class InitiativeData
{
    public int TurnNumber;
    public CharacterTurn[] TurnOrder;
    public int TurnPointer;
}


[Serializable]
public class CharacterTurn
{
    public int CharacterID;
    public bool IsFriendly;
    public int Roll;
}