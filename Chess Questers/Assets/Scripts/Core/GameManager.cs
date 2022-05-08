using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public GameStatesEnum State { get; private set; }

    private NewBattleAction[] _actions;
    private EnemySO[] _enemies;
    private Encounter[] _encounters;
    private CreatureModel[] _characterModels;

    private PlayerClass[] _playerClasses;

    private QuestJsonData _questData;


    [SerializeField] private CreatureRuntimeSet _playerCharacters;
    [SerializeField] private CreatureRuntimeSet _enemyList;
    [SerializeField] private Faction _partyFaction;

    [SerializeField] private GameObject _characterPrefab;

    [SerializeField] private bool _useTestEncounter;
    [SerializeField] private int _debugEncounterID;

    public void Awake()
    {
        _actions = Resources.LoadAll<NewBattleAction>("Scriptable Objects/Actions/");
        _enemies = Resources.LoadAll<EnemySO>("Scriptable Objects/Enemies/");
        _encounters = Resources.LoadAll<Encounter>("Scriptable Objects/BattleEncounters/");
        _characterModels = Resources.LoadAll<CreatureModel>("Scriptable Objects/Creatures/");
        _playerClasses = Resources.LoadAll<PlayerClass>("Scriptable Objects/PlayerClasses/");

        SceneManager.sceneLoaded += LoadScene;
        DontDestroyOnLoad(gameObject);

    }


    private void LoadScene(Scene scene, LoadSceneMode mode)
    {
        // based on what the scene is, load various things...
        if (scene.buildIndex == 2)
        {
            // battle encounter, find battlemanager and init?


            Debug.Log("Loading Battle scene!");
            //EventSystem.BattleStarted();

        }

    }


    //public QuestData InitQuestData()
    //{
    //    Encounter testEncounter = GetEncounter(1);

    //    List<ImprovedCharacter> Enemies = testEncounter.GetEnemies();

    //    QuestData qd = new QuestData()
    //    {
    //        Floor = 1,
    //        CurrentEncounterType = EncounterTypesEnum.Battle,
    //        Battle_ID = testEncounter.ID,
    //        Battle_Layout = testEncounter.Layout,
    //        Enemies = Enemies,
    //    };

    //    // other things to initialize can go here...
        

    //    return qd;

    //}

    //public QuestJsonData InitNewQuestData()
    //{
    //    Encounter testEncounter = GetEncounter(1);

    //    //CharacterJsonData[] Enemies = testEncounter.GetEnemiesJson();
    //    EnemyJsonData[] Enemies = testEncounter.GetEnemiesJsonNew();

    //    QuestJsonData qd = new QuestJsonData()
    //    {
    //        Floor = 1,
    //        CurrentEncounterType = EncounterTypesEnum.Battle,
    //        Battle_ID = testEncounter.ID,
    //        Battle_Layout = testEncounter.Layout,
    //        Enemies = Enemies,
    //    };

    //    // other things to initialize can go here...


    //    return qd;

    //}

    public void GetEncounter(QuestJsonData data)
    {
        Encounter encounter;
        if (_useTestEncounter)
        {
            encounter = GetEncounter(_debugEncounterID);
        }
        else
        {
            // get random seeded encounter
            // replace for real random select function!
            encounter = GetEncounter(_debugEncounterID);
        }

        data.SetNextEncounter(encounter);

    }

    public void SetupTestEncounter(QuestJsonData data)
    {
        Encounter testEncounter = GetEncounter(1);

        data.SetNextEncounter(testEncounter);    
    }





    public QuestJsonData GetQuestData()
    {
        _questData = SaveDataManager.Load();
        return _questData;
    }


    public void ContinueQuest()
    {
        _questData = SaveDataManager.Load();
        

        // depending on where the player is on the quest, load the relevant scene...
        switch (_questData.CurrentEncounterType)
        {
            case EncounterTypesEnum.Battle:
                SceneManager.LoadScene(2);
                break;
            default:
                Debug.Log($"Encounter Type {_questData.CurrentEncounterType.ToString()} ({_questData.CurrentEncounterType}) not implemented!");
                break;
        }

        

    }


    public void EndQuest()
    {
        SceneManager.LoadScene(0);
    }


    //public QuestJsonData LoadQuest(List<ImprovedCharacter> adventurers, List<ImprovedCharacter> enemies)
    //{
    //    _questData = SaveDataManager.Load();
    //    adventurers = SaveDataManager.DeserializeCharacterData(_questData.PartyMembers, true);
    //    enemies = SaveDataManager.DeserializeCharacterData(_questData.Enemies, false);

    //    return _questData;
    //}

    public void SaveQuest(QuestJsonData questData, List<PlayerCharacter> adventurers, List<Enemy> enemies)
    {
        questData.PartyMembers = SaveDataManager.SerializeCharacterData(adventurers);
        questData.Enemies = SaveDataManager.SerializeEnemyData(enemies);

        SaveDataManager.Save(questData);
    }

    public void SaveQuestNew(QuestJsonData questData, List<Creature> adventurers, List<Creature> enemies)
    {
        questData.PartyMembers = SaveDataManager.SerializeCharacterDataNew(adventurers);
        questData.Enemies = SaveDataManager.SerializeEnemyDataNew(enemies);

        SaveDataManager.Save(questData);
    }

    public Encounter GetEncounter(int encounterID)
    {
        return _encounters.Where(w => w.ID == encounterID).Single();
    }

    #region Battle Encounters

    public void LostBattle()
    {

    }


    public void WonBattle()
    {

    }

    #endregion

    #region Character

    public PlayerClass GetRandomPlayerClass()
    {
        if (_playerClasses.Length == 0)
            return null;

        int index = Random.Range(0, _playerClasses.Length);
        return _playerClasses[index];

    }


    //public MoveClass GetRandomMoveClass()
    //{
    //    if (_moveClasses.Length == 0)
    //        return null;

    //    var playerMoveClasses = _moveClasses.Where(w => w.ForPlayer).ToArray();

    //    int index = Random.Range(0, playerMoveClasses.Length);
    //    return playerMoveClasses[index];

    //}

    //public MoveClass GetMoveClassWithID(int id)
    //{
    //    return _moveClasses.Where(w => w.ID == id).Single();
    //}




    public NewBattleAction GetActionNew(int actionID)
    {
        return _actions.Where(w => w.ID == actionID).Single();
    }

    #endregion


    #region Enemies

    public EnemySO GetEnemyObject(int enemyID)
    {
        return _enemies.Where(w => w.ID == enemyID).Single();
    }

    #endregion


    #region Creature Models

    public GameObject GetCreatureModelPrefab(int modelID)
    {
        return _characterPrefab;

        //var model = _creatureModels.Where(w => w.ID == modelID).Single();

        //return model.ModelPrefab;
    }


    public CreatureModel GetCreatureModel(int creatureModelID)
    {
        return _characterModels.Where(w => w.ID == creatureModelID).Single();
    }

    //public Dictionary<int, GameObject> GetCreatureModelPrefabs()
    //{
    //    Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();

    //    foreach (var creatureModel in _creatureModels)
    //    {
    //        int id = creatureModel.ID;
    //        var obj = Instantiate(creatureModel.ModelPrefab);
    //        prefabs.Add(id, obj);
    //    }

    //    return prefabs;
    //}

    public int GetRandomCreatureModelID()
    {
        if (_characterModels.Length == 0)
            return 0;

        int index = Random.Range(0, _characterModels.Length);
        return _characterModels[index].ID;

    }

    #endregion


    public enum GameStatesEnum
    {
        Start,
        World,
        Battle_Start,
        Battle_Victory,
        Battle_Defeat,
        Shop,
        Campsite,
        Game_Over
    }

}
