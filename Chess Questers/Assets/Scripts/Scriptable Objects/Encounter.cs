using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Chess Questers/Battle Encounter")]
public class Encounter : ScriptableObject
{
    public int ID;
    public int Difficulty;
    public int Type;

    public int Layout;
    public Enemy[] Enemies;

    public Vector2[] PlayerSpawns;
    public Vector2[] EnemySpawns;


    public CharacterJsonData[] GetEnemiesJson()
    {
        
        CharacterJsonData[] toReturn = new CharacterJsonData[Enemies.Length];

        for (int i = 0; i < Enemies.Length; i++)
        {
            toReturn[i] = new CharacterJsonData(Enemies[i]);
        }

        return toReturn;

    }

    //public List<ImprovedCharacter> GetEnemies()
    //{
    //    List<ImprovedCharacter> toReturn = new List<ImprovedCharacter>();

    //    foreach (var e in Enemies)
    //    {
    //        toReturn.Add(new ImprovedCharacter(e));
    //    }

    //    return toReturn;
    //}

}
