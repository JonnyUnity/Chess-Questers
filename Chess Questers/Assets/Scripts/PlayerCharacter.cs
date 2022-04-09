using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter : Creature
{

    public Color MoveClassColor => MoveClass.DebugColor;

    private ActionClass _selectedAction;

    protected override void OnEnable()
    {
        base.OnEnable();
        BattleEvents.OnPlayerActionSelected += SetSelectedAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BattleEvents.OnPlayerActionSelected -= SetSelectedAction;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (State != CharacterStatesEnum.MOVING) return;

        Vector3 direction = (TargetPosition - Transform.position).normalized;
        Transform.position += MoveSpeed * Time.deltaTime * direction;

        if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
        {
            Transform.SetPositionAndRotation(TargetPosition, _orientation);

            //SetPosition(TargetX, TargetY);
            State = CharacterStatesEnum.IDLE;
        }
    }


    public void InitFromCharacterData(CharacterJsonData data)
    {
        ID = data.ID;
        Name = data.Name;
        _nameText.text = Name;
        CharacterModel = data.CharacterModel;
        IsFriendly = true;
        MoveClass = GameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveClassText = MoveClass.name;

        Actions = GameManager.Instance.GetActionsWithIDs(data.Actions);
        Health = data.Health;
        MaxHealth = data.MaxHealth;

        CellX = data.CellX;
        CellY = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }

    public void SetSelectedAction(int characterID, ActionClass action, int x, int y)
    {
        if (ID != characterID)
            return;

        _selectedAction = Actions.Where(w => w.ID == action.ID).Single();
    }

    public override int GetAttackDamage()
    {
        // action base damage
        // (+  any modifiers?)
        return _selectedAction.Damage;
    }

}
