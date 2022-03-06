using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureActionHandler : MonoBehaviour
{

    [SerializeField] private AttackClass[] _actions;
    private AttackClass _selectedAction;


    public AttackClass[] GetActions()
    {
        return _actions;
    }


    public void SetSelectedAction(AttackClass selectedAction)
    {
        // only set it as selected if it exists amongst the list of attacks.
        if (Array.IndexOf(_actions, selectedAction) > 0)
        {
            _selectedAction = selectedAction;
        }
    }

    public AttackClass GetSelectedAction()
    {
        return _selectedAction;
    }

}
