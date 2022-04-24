using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InitiativeManager : MonoBehaviour
{

    [SerializeField] private GameObject _portraitsContainer;
    [SerializeField] private GameObject _portraitImagePrefab;

    [SerializeField] private CreatureRuntimeSet _playerCharacters;
    [SerializeField] private CreatureRuntimeSet _enemies;
    //[SerializeField] private CreatureRuntimeSet _combatants;
    [SerializeField] private IntVariable TurnNumber;
    [SerializeField] private IntVariable TurnPointer;

    [SerializeField] private float deathPortraitAnimationTime;

    [SerializeField] private InitiativeSet _initiative;

    private List<CreaturePortrait> _creaturePortraits = new List<CreaturePortrait>();

    //public int ActiveCharacterID
    //{
    //    get
    //    {
    //        return _combatants.Items[TurnPointer.Value].ID;
    //    }
    //}


    private void OnEnable()
    {
        BattleEvents.OnStartCombat += Roll;
        BattleEvents.OnTurnOver += NextTurn;
        BattleEvents.OnDeath += CharacterDied;
        BattleEvents.OnResumeCombat += Setup;
    }

    private void OnDisable()
    {
        BattleEvents.OnStartCombat -= Roll;
        BattleEvents.OnTurnOver -= NextTurn;
        BattleEvents.OnDeath -= CharacterDied;
        BattleEvents.OnResumeCombat -= Setup;
    }

    private void Setup()
    {
        _initiative.Empty();

        foreach (var creature in _playerCharacters.Items)
        {
            //_combatants.Add(creature);
            _initiative.Add(creature);
        }
        foreach (var creature in _enemies.Items)
        {
            //_combatants.Add(creature);
            _initiative.Add(creature);
        }

        // setup portraits
        SetupPortraits();

        //BattleEvents.TurnStarted(ActiveCharacterID);
        BattleEvents.TurnStarted(_initiative.ActiveCharacter);

    }

    private void Roll()
    {
        _initiative.Empty();

        foreach (var creature in _playerCharacters.Items)
        {
            creature.SetInitiative(Random.Range(1, 20));
            //_combatants.Add(creature);
            _initiative.Add(creature);
        }

        foreach (var creature in _enemies.Items)
        {
            creature.SetInitiative(Random.Range(1, 20));
            //_combatants.Add(creature);
            _initiative.Add(creature);
        }

        _initiative.Sort();
        //_combatants.Sort();
        SetupPortraits();

        TurnNumber.SetValue(1);

        //BattleEvents.TurnStarted(ActiveCharacterID);
        BattleEvents.TurnStarted(_initiative.ActiveCharacter);

    }


    private void NextTurn()
    {
        _initiative.ActiveCharacter.UpdateActionCooldowns();

        Debug.Log("Initiative - NextTurn");
        TurnPointer.Inc();
        
        if (TurnPointer.Value > _initiative.Items.Count() - 1)
        {
            TurnNumber.Inc();
            TurnPointer.SetValue(0);
            Debug.Log("Initiative - Increase the turn number!");
        }

        UpdatePortraitsOfNextTurn();

        //BattleEvents.TurnStarted(ActiveCharacterID);
        BattleEvents.TurnStarted(_initiative.ActiveCharacter);
    }


    private void CharacterDied(Creature creature)
    {
        Debug.Log("Someone died! Update the turn order!");

        RemovePortrait(creature.ID);
        //_combatants.Remove(creature);
        _initiative.Remove(creature);
        

        if (_initiative.AllEnemiesDead)
        {
            BattleEvents.Victory();
        }
        if (_initiative.AllFriendliesDead)
        {
            BattleEvents.Loss();
        }

        //if (!_combatants.Items.Any(a => !a.IsFriendly))
        //{
        //    BattleEvents.Victory();
        //}
        //if (!_combatants.Items.Any(a => a.IsFriendly))
        //{
        //    BattleEvents.Loss();
        //}
    }


    #region Portrait UI Functions

    private void SetupPortraits()
    {
        foreach (var combatant in _initiative.Items)
        {
            GameObject portraitObj = Instantiate(_portraitImagePrefab, _portraitsContainer.transform);
            CreaturePortrait portrait = portraitObj.GetComponent<CreaturePortrait>();

            portrait.SetPortrait(combatant);
            _creaturePortraits.Add(portrait);

        }
    }


    private void UpdatePortraitsOfNextTurn()
    {
        var portraitObj = _portraitsContainer.transform.GetChild(0).gameObject;

        portraitObj.transform.parent = null;
        //portraitObj.transform.parent = _portraitsContainer.transform;
        portraitObj.transform.SetParent(_portraitsContainer.transform);
    
    }

    private void RemovePortrait(int creatureID)
    {
        CreaturePortrait deadCreature = _creaturePortraits.Where(w => w.ID == creatureID).Single();

        //deadCreature.gameObject.transform.parent = null;

        StartCoroutine(RemovePortraitCoroutine(deadCreature.gameObject));

    }

    private IEnumerator RemovePortraitCoroutine(GameObject portraitObj)
    {

        Animator animator = portraitObj.GetComponent<Animator>();
        //portraitObj.transform.parent = null;
        animator.SetBool("HasDied", true);

        yield return new WaitForSeconds(1f);

        Destroy(portraitObj);
    }


    #endregion


}
