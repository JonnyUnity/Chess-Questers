using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter : Creature
{

    public Color MoveClassColor => MoveAction.DebugColor;

    

    private NewBattleAction _selectedAction;

    [SerializeField] private CreatureRuntimeSet _party;
    [SerializeField] private Faction _partyFaction;

    [SerializeField] private GameObject[] _characterModels;

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


    //public void DoMove(Vector3 position, int x, int y)
    //{
    //    TargetX = x;
    //    TargetY = y;

    //    TargetPosition = position;
    //    State = CharacterStatesEnum.MOVING;
    //}

    
    public void InitFromCharacterData(CharacterJsonData data)
    {
        ID = data.ID;
        Name = data.Name;
        //_nameText.text = Name;
        CreatureModelID = data.CreatureModelID;
        SetCharacterModel(CreatureModelID);

        IsFriendly = true;
        ActionsPerTurn = data.ActionsPerTurn;
        ActionsRemaining = data.ActionsRemaining;

        MoveAction = Instantiate(GameManager.Instance.GetActionNew(data.MoveActionID));
        MoveClassText = MoveAction.name;

        _creatureModel = GameManager.Instance.GetCreatureModel(CreatureModelID);
        _portraitSprite = _creatureModel.Portrait;

        SetInitiative(data.Initiative);

        foreach (var jsonAction in data.BattleActions)
        {
            var actionRef = GameManager.Instance.GetActionNew(jsonAction.ID);
            NewBattleAction action = Instantiate(actionRef);
            action.Init(jsonAction, _partyFaction);

            Actions.Add(action);
        }

        Health = data.Health;
        MaxHealth = data.MaxHealth;

        X = data.CellX;
        Y = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }

    public void SetPartySelectMode()
    {
        State = CharacterStatesEnum.PARTY_SELECT;
    }



    public void SetCharacterModel(int modelID)
    {
        foreach (var characterModel in _characterModels)
        {
            characterModel.SetActive(false);
        }
        _characterModels[modelID].SetActive(true);
        _materialObject = _characterModels[modelID];
    }


    public void DoSelectedAction(ActionResult actionResult)
    {
        StartCoroutine(SelectedActionCoroutine(actionResult));
    }



    private IEnumerator SelectedActionCoroutine(ActionResult actionResult)
    {
        //Debug.Log("Start action");

        //BattleEvents.ActionStarted(actionResult.Action);

        yield return StartCoroutine(DoTurnCoroutine(actionResult));

        //Debug.Log("Finish action");
        //BattleEvents.ActionFinished();


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
