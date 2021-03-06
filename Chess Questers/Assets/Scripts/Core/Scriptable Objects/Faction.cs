using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Chess Questers/Faction")]
    public class Faction : ScriptableObject
    {
        public CreatureRuntimeSet Friendlies;
        public CreatureRuntimeSet NonFriendlies;

        public CreatureRuntimeSet GetTargetFaction(bool isFriendly)
        {
            return isFriendly ? Friendlies : NonFriendlies;
        }

    }
}