using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitiativeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _turnInfo;

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

        _turnInfo.text = "TURN " + TurnNumber.Value.ToString();

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
        _turnInfo.text = "TURN " + TurnNumber.Value.ToString();

        BattleEvents.TurnStarted(_initiative.ActiveCharacter);

    }


    private void NextTurn()
    {
        _initiative.ActiveCharacter.UpdateActionCooldowns();

        Debug.Log("Initiative - NextTurn " + TurnPointer.Value);
        TurnPointer.Inc();
        
        if (TurnPointer.Value > _initiative.Items.Count() - 1)
        {
            TurnNumber.Inc();
            TurnPointer.SetValue(0);

            _turnInfo.text = "TURN " + TurnNumber.Value.ToString();
           // Debug.Log("Initiative - Increase the turn number!");
        }

        UpdatePortraitsOfNextTurn();

        

        BattleEvents.TurnStarted(_initiative.ActiveCharacter);
    }


    private void CharacterDied(Creature creature)
    {

        StartCoroutine(CreatureDiedCoroutine(creature));

        //// move to a co-routine to get a proper pause after 

        //RemovePortrait(creature.ID);
        
        //// Move the turn pointer back if a creature earlier in the turn dies.
        //int index = _initiative.Items.IndexOf(creature);
        //if (index <= TurnPointer.Value)
        //{
        //    TurnPointer.Dec();
        //}
        //_initiative.Remove(creature);
        
        //if (_initiative.AllEnemiesDead)
        //{
        //    BattleEvents.Victory();
        //}
        //if (_initiative.AllFriendliesDead)
        //{
        //    BattleEvents.Loss();
        //}

    }


    private IEnumerator CreatureDiedCoroutine(Creature creature)
    {
        Debug.Log("Someone died! Update the turn order!");

        // move to a co-routine to get a proper pause after 

        //RemovePortrait(creature.ID);
        

        // Move the turn pointer back if a creature earlier in the turn dies.
        
        int index = _initiative.Items.IndexOf(creature);
        Debug.Log("Creature died " + creature.Name + " - " + TurnPointer.Value + " <= " + index);
        if (index <= TurnPointer.Value)
        {
            TurnPointer.Dec();
        }
        _initiative.Remove(creature);

        Debug.Log("Remove portrait start");
        CreaturePortrait deadCreature = _creaturePortraits.Where(w => w.ID == creature.ID).Single();
        yield return StartCoroutine(RemovePortraitCoroutine(deadCreature.gameObject));
        Debug.Log("Remove portrait finish");

        if (_initiative.AllEnemiesDead)
        {
            yield return new WaitForSeconds(1f);
            BattleEvents.Victory();
        }
        if (_initiative.AllFriendliesDead)
        {
            yield return new WaitForSeconds(1f);
            BattleEvents.Loss();
        }

        yield return null;
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

        portraitObj.transform.SetParent(null);
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

        yield return new WaitForSeconds(1f);

        LeanTween.moveLocalY(portraitObj, -150f, 1f);
        LeanTween.alphaCanvas(portraitObj.GetComponent<CanvasGroup>(), 0, 1f).setDelay(0.1f).setOnComplete(() => Destroy(portraitObj));

        //Destroy(portraitObj);
    }

    #endregion

}