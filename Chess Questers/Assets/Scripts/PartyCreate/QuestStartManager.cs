using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestStartManager : Singleton<QuestStartManager>
{

    [SerializeField] private string _namesText;
    private string[] _characterNames;

    [Header("Character Game Objects")]
    [SerializeField] private GameObject _character1GameObject;
    [SerializeField] private GameObject _character2GameObject;
    [SerializeField] private GameObject _character3GameObject;

    [Header("Character UI Managers")]
    [SerializeField] private CharacterSelectManager _character1Manager;
    [SerializeField] private CharacterSelectManager _character2Manager;
    [SerializeField] private CharacterSelectManager _character3Manager;

    //private QuestData _questData;
    private QuestJsonData _newData;

    //private ImprovedCharacter[] Characters;
    private CharacterJsonData[] NewChars;

    private CreatureModel[] _creatureModels;

    private Dictionary<int, GameObject> _creatureModelPrefabs = new Dictionary<int, GameObject>(); 


    void Start()
    {
        NewChars = new CharacterJsonData[3];
        //Characters = new ImprovedCharacter[3];
        _characterNames = _namesText.Split(" ");
        //_creatureModelPrefabs = GameManager.Instance.GetCreatureModelPrefabs();        

        _newData = new QuestJsonData();

        //_character1Manager.SetCharacterModel(_character1GameObject);
        //_character2Manager.SetCharacterModel(_character2GameObject);
        //_character3Manager.SetCharacterModel(_character3GameObject);

        _character1Manager.RerollCharacter();
        _character2Manager.RerollCharacter();
        _character3Manager.RerollCharacter();

    }


    public CharacterJsonData RandomiseCharacter(int characterSlot)
    {
        // generate random name..

        // select random move class

        // select actions (weighted based on move class)

        // select random character model...
        MoveClass mc = GameManager.Instance.GetRandomMoveClass();
        List<ActionClass> actions = GameManager.Instance.GetActions(mc);

        int nameIndex = Random.Range(0, _characterNames.Length);
        string charName = _characterNames[nameIndex];
        int creatureModelID = GameManager.Instance.GetRandomCreatureModelID();

        CharacterJsonData newChar = new CharacterJsonData(charName, creatureModelID, mc.ID, actions, 100);

        NewChars[characterSlot] = newChar;
        
        return newChar;

    }

    public GameObject GetModelPrefab(int id)
    {
        if (_creatureModelPrefabs.ContainsKey(id))
        {
            return _creatureModelPrefabs[id];
        }

        return null;
    }


    public void AddModelPrefabToPool(int id, GameObject prefab)
    {
        _creatureModelPrefabs.Add(id, prefab);
    }



    public void StartQuest()
    {

        _newData.PartyMembers = NewChars;
        GameManager.Instance.SetupTestEncounter(_newData);
        SaveDataManager.Save(_newData);

        // randomise world map?

        // go to map scene


        SceneManager.LoadScene(2);
    }


    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }

}