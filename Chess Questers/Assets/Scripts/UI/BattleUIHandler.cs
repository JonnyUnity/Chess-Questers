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

    private int _currentCharacterID;
    private ActionClass[] _currentActions;
    private int _characterX;
    private int _characterY;


    private void Awake()
    {
        BattleEvents.OnCellAttackSelected += AttackSelected;
    }

    private void AttackSelected(GridCell cell)
    {
        HideActions();
    }

    private void OnDestroy()
    {
        BattleEvents.OnCellAttackSelected -= AttackSelected;
    }

    public void UpdateStateText(string text)
    {
        _stateText.text = text;
    }

    public void UpdateCharacterText(string text)
    {
        _characterText.text = text;
    }


    public void ShowActions(int characterID, ActionClass[] actions, int x, int y)
    {
        _currentCharacterID = characterID;
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
        //Debug.Log(go.name + " clicked!");

        // Extract action index
        if (int.TryParse(go.name.Substring(0, 1), out int actionIndex))
        {
            ActionClass action = _currentActions[actionIndex];

            BattleEvents.ActionSelected(_currentCharacterID, action, _characterX, _characterY);

        }
    }

    public void SkipAction()
    {

        Debug.Log("Skipped action!");
        HideActions();

        BattleEvents.TurnOver();
    }


    private void HideActions()
    {
        foreach (var attackButton in _attackButtons)
        {
            attackButton.SetActive(false);
        }

    }

}