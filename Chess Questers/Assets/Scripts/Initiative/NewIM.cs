using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewIM : MonoBehaviour
{

    //private List<ImprovedCharacter> _playerCharacters;
    //private List<ImprovedCharacter> _enemies;

    public InitiativeData _init { get; private set; }

    public int ActiveCharacterID
    {
        get
        {
            return _init.TurnOrder[_init.TurnPointer].CharacterID;
        }
    }

    private void Awake()
    {
        _init = new InitiativeData();
        BattleEvents.OnRollInitiative += Roll;
        BattleEvents.OnTurnOver += NextTurn;
        BattleEvents.OnDeath += CharacterDied;
    }

    private void OnDisable()
    {
        BattleEvents.OnRollInitiative -= Roll;
        BattleEvents.OnTurnOver -= NextTurn;
        BattleEvents.OnDeath -= CharacterDied;

    }

    public bool HasCombatStarted()
    {
        if (_init.TurnNumber > 0)
        {
            return true;
        }
        else if (_init.TurnNumber == 0 && _init.TurnPointer > 0)
        {
            return true;
        }

        return false;

    }


    public void SetInitiative(InitiativeData current)
    {
        _init = current;
    }

    private void Roll(List<Creature> characters)
    {
        List<CharacterTurn> combatants = new List<CharacterTurn>();

        //int id = 1;
        characters.ForEach(a =>
        {
            //a.ID = id;
            //id++;
            combatants.Add(new CharacterTurn()
            {

                CharacterID = a.ID,
                IsFriendly = a.IsFriendly,
                Roll = Random.Range(1, 20)
            });

            //if (a.IsFriendly)
            //{
            //    _playerCharacters.Add(a);
            //}
            //else
            //{
            //    _enemies.Add(a);
            //}

        });

        _init.TurnOrder = combatants.OrderByDescending(o => o.Roll).ThenBy(t => t.CharacterID).ToArray();
        _init.TurnPointer = 0;
        _init.TurnNumber = 1;

        // Initiative all done so start battle!
        Debug.Log("Initiative - Start Battle!");
        BattleEvents.TurnStarted(_init.TurnOrder[0].CharacterID);

    }


    //public int GetActiveCharacterID()
    //{
    //    return _init.TurnOrder[_init.TurnPointer].CharacterID;
    //}


    private void NextTurn()
    {
        Debug.Log("Initiative - NextTurn");
        _init.TurnPointer++;
        if (_init.TurnPointer > _init.TurnOrder.Count() - 1)
        {
            _init.TurnNumber++;
            _init.TurnPointer = 0;
            Debug.Log("Initiative - Increase the turn number!");

        }

        BattleEvents.TurnStarted(ActiveCharacterID);
    }


    private void CharacterDied(Creature creature)
    {
        Debug.Log("Someone died! Update the turn order!");

        _init.TurnOrder = _init.TurnOrder.Where(w => w.CharacterID != creature.ID).ToArray();


        // If all enemies are dead...
        if (!_init.TurnOrder.Where(w => !w.IsFriendly).Any())
        {
            BattleEvents.Victory();
        }

        // if all party members are dead..
        if (!_init.TurnOrder.Where(w => w.IsFriendly).Any())
        {
            BattleEvents.Loss();
        }


    }

}
