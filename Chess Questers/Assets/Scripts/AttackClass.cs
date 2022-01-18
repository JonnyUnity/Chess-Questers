using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackClass : ScriptableObject
{
    public string Name;

    public int Size;
    public int Range;
    public AttackShapesEnum Shape;

    public bool MoveTarget;

    public bool ForPlayer;
    public bool ForEnemy;


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

