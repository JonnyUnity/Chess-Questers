using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private TextMeshProUGUI _characterText;


    [SerializeField] private GameObject _attackList;
    [SerializeField] private GameObject[] _attackButtons;
    [SerializeField] private GameObject PassButton;


    private ActionClass[] _currentActions;
    private int _characterX;
    private int _characterY;

    private Creature _currentCharacter;

    [SerializeField] private ActionResult _playerAction;


    private void Awake()
    {
        BattleEvents.OnCellAttackSelected += AttackSelected;
        BattleEvents.OnTurnOver += HideActions;
        BattleEvents.OnCellSelected += AttackSelected;
    }

    private void AttackSelected(GridCell cell)
    {
        //CreatureRuntimeSet creatures = _currentCharacter.Faction.GetTargetFaction(_playerAction.Action.IsAttack);

        //_playerAction.Cell = cell;
        //List<ActionResult> results = GameGrid.Instance.GetTargetsOfActionNew(_playerAction.Action, creatures, cell.X, cell.Y);
        //_playerAction.Creatures = results[0].Creatures;
        //_playerAction.Creatures = GameGrid.Instance.GetAttackedCreatures(cell, _playerAction.Action);

        HideActions();
    }

    private void OnDestroy()
    {
        BattleEvents.OnCellAttackSelected -= AttackSelected;
        BattleEvents.OnTurnOver -= HideActions;
        BattleEvents.OnCellSelected -= AttackSelected;
    }

    public void UpdateStateText(string text)
    {
        _stateText.text = text;
    }

    public void UpdateCharacterText(string text)
    {
        _characterText.text = text;
    }


    public void ShowActions(Creature currentCreature, ActionClass[] actions, int x, int y)
    {

        _currentCharacter = currentCreature;
        _currentActions = actions;
        _characterX = x;
        _characterY = y;

        HideActions();

        for (int i = 0; i < actions.Length; i++)
        {
            var buttonText = _attackButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = actions[i].Name;
            _attackButtons[i].SetActive(true);
        }

        _attackList.SetActive(true);
        PassButton.SetActive(true);

    }

    public void SelectAttack()
    {
        var go = EventSystem.current.currentSelectedGameObject;

        // Extract action index
        if (int.TryParse(go.name.Substring(0, 1), out int actionIndex))
        {
            ActionClass action = _currentActions[actionIndex];

            _playerAction.Action = action;
            _playerAction.Damage = action.Damage;

            CreatureRuntimeSet creatures = _currentCharacter.Faction.GetTargetFaction(action.IsAttack);

            BattleEvents.ActionSelected(action, creatures, _currentCharacter.CellX, _currentCharacter.CellY);

        }
    }


    private void HideActions()
    {
        foreach (var attackButton in _attackButtons)
        {
            attackButton.SetActive(false);
        }

    }

}