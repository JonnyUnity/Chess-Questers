using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : Singleton<NewGameManager>
{



    private MoveClass[] _moveClasses;
    private AttackClass[] _actionClasses;
    private Enemy[] _enemies;
    private Encounter[] _encounters;
    private Sprite[] _debugPortraitSprites;


    private QuestData _questData;

    public void Awake()
    {
        _moveClasses = Resources.LoadAll<MoveClass>("MoveClasses/");
        _actionClasses = Resources.LoadAll<AttackClass>("AttackClasses/");
        _debugPortraitSprites = Resources.LoadAll<Sprite>("Sprites/");
        _enemies = Resources.LoadAll<Enemy>("Enemies/");
        _encounters = Resources.LoadAll<Encounter>("BattleEncounters/");

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
            EventSystem.BattleStarted();

        }

    }


    public QuestData InitQuestData()
    {
        Encounter testEncounter = GetEncounter(1);

        List<ImprovedCharacter> Enemies = testEncounter.GetEnemies();

        QuestData qd = new QuestData()
        {
            Floor = 1,
            CurrentEncounterType = EncounterTypesEnum.Battle,
            Battle_ID = testEncounter.ID,
            Battle_Layout = testEncounter.Layout,
            Enemies = Enemies,
        };

        // other things to initialize can go here...
        

        return qd;

    }

    public QuestData GetQuestData()
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

    public void SaveQuest(QuestData questData)
    {
        SaveDataManager.Save(questData);
    }


    public Encounter GetEncounter(int encounterID)
    {
        return _encounters.Where(w => w.ID == encounterID).Single();
    }

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


    public AttackClass[] GetAttacks(MoveClass moveClass)
    {
        // Attacks might be limited to certain move classes...

        return _actionClasses;

    }

    public AttackClass[] GetActionsWithIDs(int[] ids)
    {
        return _actionClasses.Where(w => ids.Contains(w.ID)).ToArray();
    }

    #endregion



}
