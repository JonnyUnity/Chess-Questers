using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureActionHandler : MonoBehaviour
{

    [SerializeField] private ActionClass[] _actions;
    private ActionClass _selectedAction;


    public ActionClass[] GetActions()
    {
        return _actions;
    }


    public void SetSelectedAction(ActionClass selectedAction)
    {
        // only set it as selected if it exists amongst the list of attacks.
        if (Array.IndexOf(_actions, selectedAction) > 0)
        {
            _selectedAction = selectedAction;
        }
    }

    public ActionClass GetSelectedAction()
    {
        return _selectedAction;
    }

}
