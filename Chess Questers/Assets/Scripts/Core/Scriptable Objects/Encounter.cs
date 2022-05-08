using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Encounter", menuName = "Chess Questers/Battle Encounter")]
    public class Encounter : ScriptableObject
    {
        public int ID;
        public int Difficulty;
        public EncounterTypesEnum Type;

        public int Layout;
        public int Height;
        public int Width;

        public EnemySO[] Enemies;

        public Vector2[] PlayerSpawns;
        public Vector2[] EnemySpawns;


        public EnemyJsonData[] GetEnemiesJson()
        {
            EnemyJsonData[] toReturn = new EnemyJsonData[Enemies.Length];

            for (int i = 0; i < Enemies.Length; i++)
            {
                toReturn[i] = new EnemyJsonData(Enemies[i]);
            }

            return toReturn;
        }

    }
}