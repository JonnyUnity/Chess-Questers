using System;

[Serializable]
public class NewInitiative
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