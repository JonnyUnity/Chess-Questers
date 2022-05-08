using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess Questers/Initiative Set")]
public class InitiativeSet : CreatureRuntimeSet
{

    public IntVariable TurnNumber;
    public IntVariable TurnPointer;

    public Creature ActiveCharacter
    {
        get
        {
            if (Items.Count > 0)
            {
                return Items[TurnPointer.Value];
            }

            return null;
        }
    }

    public bool AllEnemiesDead
    {
        get
        {
            return !Items.Where(w => !w.IsFriendly).Any();
        }
    }

    public bool AllFriendliesDead
    {
        get
        {
            return !Items.Where(w => w.IsFriendly).Any();
        }
    }


}