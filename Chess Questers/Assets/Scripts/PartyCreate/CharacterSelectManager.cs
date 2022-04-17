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

    private CharacterJsonData _character;
    private GameObject _characterGameObject;
    private Renderer _renderer;

    private Transform _prefabParentTransform;

    public void Start()
    {
        _prefabParentTransform = _modelParent.transform;
    }

    public void SetCharacterModel(GameObject characterGameObject)
    {
        _characterGameObject = characterGameObject;
        //_renderer = _characterGameObject.GetComponent<Renderer>();
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

        CreatureModel creatureModel = GameManager.Instance.GetCreatureModel(_character.CreatureModelID);
        _portraitImage.sprite = creatureModel.Portrait;

        //SetCharacterModel(creatureModel.ModelPrefab);

        //var prefab = QuestStartManager.Instance.GetModelPrefab(_character.CreatureModelID);
        if (_prefabParentTransform.childCount == 1)
        {
            Destroy(_prefabParentTransform.GetChild(0).gameObject);
        }
        var modelPrefab = Instantiate(creatureModel.ModelPrefab, _prefabParentTransform);

        _moveClassText.text = moveClassText;
        _actionsText.text = "ACTIONS: " + actionsText;

        //_renderer.material.color = moveClass.DebugColor;

    }


    public void RerollCharacter()
    {
        _character = QuestStartManager.Instance.RandomiseCharacter(_characterSlot);
        SetCharacterDetails();

    }

}
