using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{

    [SerializeField] private int _characterSlot;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _moveClassText;
    [SerializeField] private TextMeshProUGUI _actionsText;

    private CharacterJsonData _character;
    private GameObject _characterGameObject;
    private Renderer _renderer;


    public void Start()
    {
    }

    public void SetCharacterModel(GameObject characterGameObject)
    {
        _characterGameObject = characterGameObject;
        _renderer = _characterGameObject.GetComponent<Renderer>();
    }

    public void SetCharacterDetails()
    {
        if (_character == null)
            return;


        _nameText.text = _character.Name;

        MoveClass moveClass = GameManager.Instance.GetMoveClassWithID(_character.MoveClassID);
        string moveClassText = moveClass.name;
        string actionsText = GameManager.Instance.GetActionNamesFromIDs(_character.Actions.Select(s => s.ID).ToArray());
        //string actionsText = string.Join(",", _character.Actions.Select(s => s.Name));

        _moveClassText.text = moveClassText;
        _actionsText.text = "ACTIONS: " + actionsText;

        _renderer.material.color = moveClass.DebugColor;

    }


    public void RerollCharacter()
    {
        _character = QuestStartManager.Instance.RandomiseCharacter(_characterSlot);
        SetCharacterDetails();

    }

}
