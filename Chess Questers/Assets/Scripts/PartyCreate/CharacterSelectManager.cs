using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{

    [SerializeField] private int _characterSlot;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _moveClassText;
    [SerializeField] private TextMeshProUGUI _actionsText;

    private ImprovedCharacter _characterData;
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
        if (_characterData == null)
            return;


        _nameText.text = _characterData.Name;
        _moveClassText.text = _characterData.MoveClassText;
        _actionsText.text = "ACTIONS: " + _characterData.ActionsText;

        _renderer.material.color = _characterData.MoveClassColor;

    }


    public void RerollCharacter()
    {
        _characterData = QuestStartManager.Instance.RandomiseCharacter(_characterSlot);
        SetCharacterDetails();

    }

}
