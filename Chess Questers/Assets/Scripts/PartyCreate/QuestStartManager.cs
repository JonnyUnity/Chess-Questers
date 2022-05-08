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

    private QuestJsonData _questData;

    private CharacterJsonData[] NewChars;

    void Start()
    {
        BattleEvents.FadeIn(() => { });

        NewChars = new CharacterJsonData[3];
        _characterNames = _namesText.Split(" ");
        _questData = new QuestJsonData();

        _character1Manager.RerollCharacter();
        _character2Manager.RerollCharacter();
        _character3Manager.RerollCharacter();

    }


    public NewCharacter RandomiseCharacter(int characterSlot)
    {
        string name = GetRandomName();
        PlayerClass playerClass = GameManager.Instance.GetRandomPlayerClass();
        int creatureModelID = GameManager.Instance.GetRandomCreatureModelID();

        NewCharacter newChar = new NewCharacter(name, playerClass, creatureModelID);

        NewChars[characterSlot] = newChar.GetJson();

        return newChar;
    }


    private string GetRandomName()
    {
        int nameIndex = Random.Range(0, _characterNames.Length);
        return _characterNames[nameIndex];
    }




    public void StartQuest()
    {

        _questData.PartyMembers = NewChars;
        //GameManager.Instance.SetupTestEncounter(_questData);
        GameManager.Instance.GetEncounter(_questData);
        SaveDataManager.Save(_questData);

        // randomise world map?

        // go to map scene

        BattleEvents.FadeOut(() => SceneManager.LoadScene(2));
        //SceneManager.LoadScene(2);
    }


    public void ReturnToTitle()
    {
        //_simpleFade.FadeOut(() => SceneManager.LoadScene(0));
        //SceneManager.LoadScene(0);

        BattleEvents.FadeOut(() => SceneManager.LoadScene(0));

    }

}