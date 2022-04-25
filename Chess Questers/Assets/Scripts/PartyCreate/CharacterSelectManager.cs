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
    [SerializeField] private TextMeshProUGUI _moveClassText;
    [SerializeField] private TextMeshProUGUI _actionsText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private GameObject _modelParent;
    [SerializeField] private ActionDisplay _actionDisplay;
    
    //private GameObject _characterGameObject;
    //private Renderer _renderer;

    private string _name;
    private int _creatureModelID;
    private PlayerClass _playerClass;


    private Transform _prefabParentTransform;

    public void Start()
    {
        _prefabParentTransform = _modelParent.transform;
    }


    public void RerollCharacter()
    {
        NewCharacter newChar = QuestStartManager.Instance.RandomiseCharacter(_characterSlot);

        CreatureModel creatureModel = GameManager.Instance.GetCreatureModel(newChar.CreatureModelID);
        _portraitImage.sprite = creatureModel.Portrait;

        if (_prefabParentTransform.childCount == 1)
        {
            Destroy(_prefabParentTransform.GetChild(0).gameObject);
        }
        var modelPrefab = Instantiate(creatureModel.ModelPrefab, _prefabParentTransform);

        _nameText.text = newChar.Name;
        _actionDisplay.SetActions(newChar.PlayerClass);

    }

}