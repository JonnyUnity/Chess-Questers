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

    // Setups the turn order when resuming a battle in progress.
    private void Setup()
    {
        _initiative.Empty();

        foreach (var creature in _playerCharacters.Items)
        {
            _initiative.Add(creature);
        }
        foreach (var creature in _enemies.Items)
        {
            _initiative.Add(creature);
        }

        _initiative.Sort();
        InitPortraits(resumingCombat:true);

        BattleEvents.TurnStarted(_initiative.ActiveCharacter);

    }

    private void Roll()
    {
        _initiative.Empty();

        foreach (var creature in _playerCharacters.Items)
        {
            creature.SetInitiative(Random.Range(1, 20));
            _initiative.Add(creature);
        }

        foreach (var creature in _enemies.Items)
        {
            creature.SetInitiative(Random.Range(1, 20));
            _initiative.Add(creature);
        }

        _initiative.Sort();
        InitPortraits();

        TurnNumber.SetValue(1);

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

        BattleEvents.TurnStarted(_initiative.ActiveCharacter);
    }


    private void CharacterDied(Creature creature)
    {
        Debug.Log("Someone died! Update the turn order!");

        RemovePortrait(creature.ID);
        
        // Move the turn pointer back if a creature earlier in the turn dies.
        int index = _initiative.Items.IndexOf(creature);
        if (index <= TurnPointer.Value)
        {
            TurnPointer.Dec();
        }
        _initiative.Remove(creature);
        
        if (_initiative.AllEnemiesDead)
        {
            BattleEvents.Victory();
        }
        if (_initiative.AllFriendliesDead)
        {
            BattleEvents.Loss();
        }

    }


    #region Portrait UI Functions

    private void InitPortraits(bool resumingCombat = false)
    {
        foreach (var combatant in _initiative.Items)
        {
            GameObject portraitObj = Instantiate(_portraitImagePrefab, _portraitsContainer.transform);
            CreaturePortrait portrait = portraitObj.GetComponent<CreaturePortrait>();

            portrait.SetPortrait(combatant);
            _creaturePortraits.Add(portrait);

        }

        if (resumingCombat)
        {
            for (int i = 0; i < TurnPointer.Value; i++)
            {
                UpdatePortraitsOfNextTurn();
            }
        }

    }


    private void UpdatePortraitsOfNextTurn()
    {
        var portraitObj = _portraitsContainer.transform.GetChild(0).gameObject;

        portraitObj.transform.parent = null;
        portraitObj.transform.SetParent(_portraitsContainer.transform);
    
    }


    private void RemovePortrait(int creatureID)
    {
        CreaturePortrait deadCreature = _creaturePortraits.Where(w => w.ID == creatureID).Single();
        StartCoroutine(RemovePortraitCoroutine(deadCreature.gameObject));
    }


    private IEnumerator RemovePortraitCoroutine(GameObject portraitObj)
    {

        Animator animator = portraitObj.GetComponent<Animator>();
        animator.SetBool("HasDied", true);

        yield return new WaitForSeconds(1f);

        Destroy(portraitObj);
    }

    #endregion

}