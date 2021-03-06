using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Model", menuName = "Chess Questers/Models")]
    public class CreatureModel : ScriptableObject
    {
        public int ID;
        public string Name;
        public string PrefabPath;

        public Sprite Portrait;
        public GameObject ModelPrefab;
    }
}