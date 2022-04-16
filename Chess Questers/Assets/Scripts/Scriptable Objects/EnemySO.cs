using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Chess Questers/Enemy")]
public class EnemySO : ScriptableObject
{
    public int ID;
    public string Name;
    public int Health;
    public int CharacterModel;

    public Brain Brain;

    public MoveClass MoveClass;
    public List<ActionClass> Actions;

}
