using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Chess Questers/Attack Class")]
public class AttackClass : ScriptableObject
{
    public int ID;
    public string Name;

    public int Size;
    public int Range;
    public AttackShapesEnum Shape;

    public bool MoveTarget;

    public bool ForPlayer;
    public bool ForEnemy;

    public Vector2[] additionalAttackedCellOffsets;

}

public enum AttackTypesEnum
{
    ADJACENT,
    LINE,
    CIRCLE,

}

public enum AttackShapesEnum
{
    POINT,
    LINE,
    CIRCLE
}

