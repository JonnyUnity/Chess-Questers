using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter : Creature
{

    public Color MoveClassColor => MoveClass.DebugColor;

    

    private ActionClass _selectedAction;

    [SerializeField] private CreatureRuntimeSet _party;
    [SerializeField] private Faction _partyFaction;

    private CreatureModel _creatureModel;

    protected override void OnEnable()
    {
        base.OnEnable();
        //BattleEvents.OnPlayerActionSelected += SetSelectedAction;
        _party.Add(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //BattleEvents.OnPlayerActionSelected -= SetSelectedAction;
        _party.Remove(this);
    }

    
    void Start()
    {
        
    }


    public void DoMove(Vector3 position, int x, int y)
    {
        TargetX = x;
        TargetY = y;

        TargetPosition = position;
        State = CharacterStatesEnum.MOVING;
    }

    
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


    public void InitFromCharacterData(CharacterJsonData data)
    {
        ID = data.ID;
        Name = data.Name;
        //_nameText.text = Name;
        CreatureModelID = data.CreatureModelID;
        IsFriendly = true;
        ActionsPerTurn = data.ActionsPerTurn;
        base.ActionsRemaining = data.ActionsRemaining;

        MoveClass = GameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveClassText = MoveClass.name;

        _creatureModel = GameManager.Instance.GetCreatureModel(CreatureModelID);
        _portraitSprite = _creatureModel.Portrait;

        SetInitiative(data.Initiative);

        foreach (var jsonAction in data.Actions)
        {

            var actionRef = GameManager.Instance.GetAction(jsonAction.ID);
            actionRef.Init(jsonAction, _partyFaction);
            ActionClass action = Instantiate(actionRef);

            Actions.Add(action);
        }

        //Actions = GameManager.Instance.GetActionsWithIDs(data.Actions.sel);
        //Actions = data.Actions;
        Health = data.Health;
        MaxHealth = data.MaxHealth;

        //_healthSlider.maxValue = MaxHealth;
        //_healthSlider.value = Health;

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
