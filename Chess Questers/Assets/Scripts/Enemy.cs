using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Creature
{
    public int EnemyID;

    public Brain Brain { get; private set; }


    public void Init(NewEnemyJsonData data, EnemySO enemyObject)
    {
        ID = data.ID;
        EnemyID = data.EnemyID;
        Name = data.Name;
        _nameText.text = Name;
        IsFriendly = false;
        CharacterModel = enemyObject.CharacterModel;
        //MoveClass = GameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveClass = enemyObject.MoveClass;
        MoveClassText = MoveClass.name;

        //Actions = GameManager.Instance.GetActionsWithIDs(data.Actions);
        Actions = enemyObject.Actions;
        Health = data.Health;
        MaxHealth = data.MaxHealth;

        _healthSlider.maxValue = MaxHealth;
        _healthSlider.value = MaxHealth;

        Brain = enemyObject.Brain;

        CellX = data.CellX;
        CellY = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }



    public void CalcMove()
    {

        GridCell move = Brain.GetMove(this);

        TargetX = move.X;
        TargetY = move.Y;
        TargetPosition = move.Position;

        State = CharacterStatesEnum.MOVING;

    }


    public EnemyAction CalcAttack()
    {
        EnemyAction enemyAction = Brain.GetAction(this);

        return enemyAction;

    }



    // Update is called once per frame
    //protected override void Update()
    //{
    //    base.Update();

    //    if (State != CharacterStatesEnum.MOVING) return;

    //    Vector3 direction = (TargetPosition - Transform.position).normalized;
    //    Transform.position += MoveSpeed * Time.deltaTime * direction;

    //    if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
    //    {
    //        Transform.SetPositionAndRotation(TargetPosition, _orientation);

    //        //SetPosition(TargetX, TargetY);
    //        State = CharacterStatesEnum.IDLE;
    //    }
    //}
}
