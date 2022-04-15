using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Chess Questers/Action Class")]
public class ActionClass : ScriptableObject
{
    public int ID;
    public string Name;

    public int Damage;
    public int MinRange;
    public int MaxRange;

    public GameObject AttackTemplatePrefab;

    public ActionShapesEnum Shape;

    public bool IsRanged;
    public bool IsAttack;

    public bool MoveTarget;

    public bool ForPlayer;
    public bool ForEnemy;

    public Vector2[] additionalAttackedCellOffsets;

}

public enum ActionTypesEnum
{
    ADJACENT,
    LINE,
    CIRCLE,

}

public enum ActionShapesEnum
{
    POINT,
    LINE,
    CIRCLE
}

