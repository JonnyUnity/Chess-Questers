using System;

[Serializable]
public class InitiativeJsonData
{
    public int TurnNumber;
    public int TurnPointer;
    public CharacterTurn[] TurnOrder;
}