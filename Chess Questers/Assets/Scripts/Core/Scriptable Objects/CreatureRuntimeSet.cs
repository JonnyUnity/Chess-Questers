using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Chess Questers/Creature Set")]
    public class CreatureRuntimeSet : RuntimeSet<Creature>
    {
        public void Sort()
        {
            Items = Items.OrderBy(o => o.Initiative).ThenBy(t => t.ID).ToList();
        }
    }
}
