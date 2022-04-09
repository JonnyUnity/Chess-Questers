using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public GameStatesEnum State { get; private set; }

    private MoveClass[] _moveClasses;
    private ActionClass[] _actionClasses;
    private EnemySO[] _enemies;
    private Encounter[] _encounters;
    private Sprite[] _debugPortraitSprites;
    private CreatureModel[] _creatureModels;


    //private QuestData _questData;
    private QuestJsonData _questData;

    public void Awake()
    {
        _moveClasses = Resources.LoadAll<MoveClass>("MoveClasses/");
        _actionClasses = Resources.LoadAll<ActionClass>("ActionClasses/");
        _debugPortraitSprites = Resources.LoadAll<Sprite>("Sprites/");
        _enemies = Resources.LoadAll<EnemySO>("Enemies/");
        _encounters = Resources.LoadAll<Encounter>("BattleEncounters/");
        _creatureModels = Resources.LoadAll<CreatureModel>("Creatures/");

        SceneManager.sceneLoaded += LoadScene;
        //EventSystem.OnBattleLoss += LostBattle;
        //EventSystem.OnBattleVictory += WonBattle;

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

    public MoveClass GetRandomMoveClass()
    {
        if (_moveClasses.Length == 0)
            return null;

        var playerMoveClasses = _moveClasses.Where(w => w.ForPlayer).ToArray();

        int index = Random.Range(0, playerMoveClasses.Length);
        return playerMoveClasses[index];

    }

    public MoveClass GetMoveClassWithID(int id)
    {
        return _moveClasses.Where(w => w.ID == id).Single();
    }


    public ActionClass[] GetAttacks(MoveClass moveClass)
    {
        // Attacks might be limited to certain move classes...

        return _actionClasses;

    }

    public int[] GetActionIDs(MoveClass moveClass)
    {
        return _actionClasses.Select(s => s.ID).ToArray();
    }


    public ActionClass[] GetActionsWithIDs(int[] ids)
    {
        return _actionClasses.Where(w => ids.Contains(w.ID)).ToArray();
    }

    public string GetActionNamesFromIDs(int[] ids)
    {
        var list = _actionClasses.Where(w => ids.Contains(w.ID)).Select(s => s.Name).ToArray();
        return string.Join(",", list);
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
        var model = _creatureModels.Where(w => w.ID == modelID).Single();

        return model.ModelPrefab;
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
