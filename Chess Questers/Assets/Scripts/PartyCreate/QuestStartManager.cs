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


    void Start()
    {
        NewChars = new CharacterJsonData[3];
        //Characters = new ImprovedCharacter[3];
        _characterNames = _namesText.Split(" ");

        //_questData = GameManager.Instance.InitQuestData();
        _newData = GameManager.Instance.InitNewQuestData();

        _character1Manager.SetCharacterModel(_character1GameObject);
        _character2Manager.SetCharacterModel(_character2GameObject);
        _character3Manager.SetCharacterModel(_character3GameObject);

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
        int[] actions = GameManager.Instance.GetActionIDs(mc);

        int nameIndex = Random.Range(0, _characterNames.Length);
        string charName = _characterNames[nameIndex];

        //ImprovedCharacter newCharacter = new ImprovedCharacter(charName, 1, mc, actions, 10);
        CharacterJsonData newChar = new CharacterJsonData(charName, 1, mc.ID, actions, 10);

        NewChars[characterSlot] = newChar;
        
        return newChar;

    }



    public void StartQuest()
    {

        // create save file with character data
        //_questData.PartyMembers = new List<ImprovedCharacter>(Characters);

        //SaveDataManager.Save(_questData);

        _newData.PartyMembers = NewChars;
        SaveDataManager.SaveNew(_newData);

        // randomise world map?

        // go to map scene


        SceneManager.LoadScene(2);
    }


    public void ReturnToTitle()
    {

        SceneManager.LoadScene(0);
    }

}