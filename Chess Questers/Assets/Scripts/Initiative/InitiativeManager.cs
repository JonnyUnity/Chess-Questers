using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitiativeManager : MonoBehaviour
{

    [SerializeField] private CreatureRuntimeSet _playerCharacters;
    [SerializeField] private CreatureRuntimeSet _enemies;
    [SerializeField] private CreatureRuntimeSet _combatants;


    [SerializeField] private IntVariable TurnNumber;
    [SerializeField] private IntVariable TurnPointer;

    public int ActiveCharacterID
    {
        get
        {
            return _combatants.Items[TurnPointer.Value].ID;
        }
    }


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
        foreach (var creature in _playerCharacters.Items)
        {
            _combatants.Add(creature);
        }
        foreach (var creature in _enemies.Items)
        {
            _combatants.Add(creature);
        }

        BattleEvents.TurnStarted(ActiveCharacterID);

    }

    private void Roll()
    {
        foreach (var creature in _playerCharacters.Items)
        {
            creature.SetInitiative(Random.Range(1, 20));
            _combatants.Add(creature);
        }

        foreach (var creature in _enemies.Items)
        {
            creature.SetInitiative(Random.Range(1, 20));
            _combatants.Add(creature);
        }

        _combatants.Sort();

        TurnNumber.SetValue(1);

        BattleEvents.TurnStarted(ActiveCharacterID);

    }


    private void NextTurn(Creature creature)
    {
        Debug.Log("Initiative - NextTurn");
        TurnPointer.Inc();
        
        if (TurnPointer.Value > _combatants.Items.Count() - 1)
        {
            TurnNumber.Inc();
            TurnPointer.SetValue(0);
            Debug.Log("Initiative - Increase the turn number!");
        }

        BattleEvents.TurnStarted(ActiveCharacterID);
    }


    private void CharacterDied(Creature creature)
    {
        Debug.Log("Someone died! Update the turn order!");

        _combatants.Items.Remove(creature);

        if (!_combatants.Items.Any(a => !a.IsFriendly))
        {
            BattleEvents.Victory();
        }
        if (!_combatants.Items.Any(a => a.IsFriendly))
        {
            BattleEvents.Loss();
        }
    }


}
