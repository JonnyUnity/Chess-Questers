using JFlex.ChessQuesters.Core.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridOracle : MonoBehaviour
{
    [SerializeField] private InitiativeSet _allCreatures;
    [SerializeField] private CreatureRuntimeSet _playerCharacters;
    [SerializeField] private CreatureRuntimeSet _enemies;

    private CreatureGraph _graph; 

    private void OnEnable()
    {
        BattleEvents.OnStartCombat += Init;
        BattleEvents.OnResumeCombat += Init;
        BattleEvents.OnCreatureMoved += UpdateDistances;
    }

    private void OnDisable()
    {
        BattleEvents.OnStartCombat -= Init;
        BattleEvents.OnResumeCombat -= Init;
        BattleEvents.OnCreatureMoved -= UpdateDistances;
    }

    public void Init()
    {

        _graph = new CreatureGraph();
        foreach (Creature creature in _allCreatures.Items)
        {
            foreach (Creature otherCreature in _allCreatures.Items.Where(w => w.ID != creature.ID))
            {
                int distance = GridUtility.CalculateChebyshevDistance(creature.X, otherCreature.X, creature.Y, otherCreature.Y);
                _graph.AddEdge(creature, otherCreature, distance);
            }
        }

        //_graph.DebugAll();

    }


    private void UpdateDistances(Creature creature)
    {

        foreach (Creature otherCreature in _allCreatures.Items.Where(w => w.ID != creature.ID))
        {
            int distance = GridUtility.CalculateChebyshevDistance(creature.X, otherCreature.X, creature.Y, otherCreature.Y);
            _graph.UpdateEdge(creature, otherCreature, distance);
            _graph.UpdateEdge(otherCreature, creature, distance);
        }

        //_graph.DebugAll();

    }


    public Creature GetClosestEnemy(Creature creature)
    {
        return null;
    }

}
