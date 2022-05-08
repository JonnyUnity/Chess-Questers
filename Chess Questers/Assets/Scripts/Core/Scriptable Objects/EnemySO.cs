using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Chess Questers/Enemy")]
    public class EnemySO : ScriptableObject
    {
        public int ID;
        public string Name;
        public int Health;
        public int ActionsPerTurn;

        public int CreatureModelID;

        public GameObject ModelPrefab;
        public Sprite Portrait;

        public Brain Brain;

        public Faction Faction;

        public NewBattleAction MoveAction;
        public List<NewBattleAction> Actions;

    }
}