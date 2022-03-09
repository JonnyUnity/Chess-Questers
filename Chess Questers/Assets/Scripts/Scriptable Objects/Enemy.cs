using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Chess Questers/Enemy")]
public class Enemy : ScriptableObject
{
    public int ID;
    public string Name;
    public int Health;
    public int CharacterModel;

    public MoveClass MoveClass;
    public ActionClass[] Actions;

}
