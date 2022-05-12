using JFlex.ChessQuesters.Core;
using JFlex.ChessQuesters.Core.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{

    [SerializeField] private int _characterSlot;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _classText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private GameObject _modelParent;
    [SerializeField] private ActionDisplay _actionDisplay;
    [SerializeField] private GameObject _characterPrefab;

    private GameObject _characterGameObject;
    private PlayerCharacter _playerCharacer;
    private Transform _prefabParentTransform;

    public void Start()
    {
        _prefabParentTransform = _modelParent.transform;
        _characterGameObject = Instantiate(_characterPrefab, _prefabParentTransform);
        _playerCharacer = _characterGameObject.GetComponent<PlayerCharacter>();
        _playerCharacer.SetPartySelectMode();
    }


    public void RerollCharacter()
    {
        NewCharacter newChar = QuestStartManager.Instance.RandomiseCharacter(_characterSlot);

        CreatureModel creatureModel = GameManager.Instance.GetCreatureModel(newChar.CreatureModelID);
        _portraitImage.sprite = creatureModel.Portrait;
        _playerCharacer.SetCharacterModel(creatureModel.ID);

        _nameText.text = newChar.Name;
        _classText.text = $"the {newChar.PlayerClass.Name}";
        _actionDisplay.SetActions(newChar.PlayerClass, _characterSlot);

    }

}