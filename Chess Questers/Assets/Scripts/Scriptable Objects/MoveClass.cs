using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Chess Questers/Move Class")]
public class MoveClass : ScriptableObject
{
    public int ID;
    public MoveTypeEnum MoveType;
    public Color DebugColor;
    public int MoveLimit = int.MaxValue;
    public bool IsJumpingPiece;
    public bool CanRam;
    public bool CanBePromoted;
    public bool ForPlayer;
    public bool ForEnemy;

}
