using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{

    private Brain _brain;
        
    public ImprovedCharacter Character { get; private set; }

    public GameGrid Grid { get; private set; }


    private void Awake()
    {
        Grid = BattleSystem.Instance.Grid;
        Character = gameObject.GetComponent<ImprovedCharacter>();
    }

    public void Init(GameGrid grid, Brain brain)
    {
        Grid = grid;
        _brain = brain;
    }


    private void OnEnable()
    {
        BattleEvents.OnEnemySelectMove += SelectMove;
        BattleEvents.OnEnemySelectAttack += SelectAttack;
    }

    private void OnDisable()
    {
        BattleEvents.OnEnemySelectMove -= SelectMove;
        BattleEvents.OnEnemySelectAttack -= SelectAttack;
    }

    private void SelectAttack(int characterID)
    {
        throw new NotImplementedException();


    }

    private void SelectMove(int characterID)
    {

        _brain.GetMove(this);

        throw new NotImplementedException();
    }
}
