using JFlex.ChessQuesters.Core.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace JFlex.ChessQuesters.Core
{
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
            _encounters = Resources.LoadAll<Encounter>("Scriptable Objects/Encounters/");
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
                Debug.Log("Loading Battle scene!");
            }

           
        }

        public void StartQuest()
        {
            // seed the map

            _questData = GetQuestData();
            //_questData.GoToMap();

            //GoToMap();
            //GoToEncounterType(EncounterTypesEnum.Battle);
            GetEncounter(_questData);
            SaveDataManager.Save(_questData);
            GoToEncounterType(EncounterTypesEnum.Battle);

        }


        public void GoToMap()
        {
            //EndQuest();
            SceneManager.LoadScene("Map");
        }


        public void GoToEncounterType(EncounterTypesEnum encounterType)
        {

            _questData = GetQuestData();


            if (encounterType == EncounterTypesEnum.Battle)
            {
                // get next encounter randomly...

                SceneManager.LoadScene(2);

            }
            else
            {
                _questData.CurrentEncounterType = encounterType;
                SaveDataManager.Save(_questData);
                SceneManager.LoadScene(encounterType.ToString());

            }

        }

        public void GoToEncounter(Encounter encounter)
        {
            _questData = GetQuestData();
            _questData.SetNextEncounter(encounter);

            SaveDataManager.Save(_questData);
            ContinueQuest();
        }




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


        public QuestJsonData GetQuestData()
        {
            _questData = SaveDataManager.Load();
            return _questData;
        }


        public void ContinueQuest()
        {
            _questData = SaveDataManager.Load();
            GoToEncounterType(_questData.CurrentEncounterType);
        }


        public void EndQuest()
        {
            //SceneManager.LoadScene(0);
            SceneManager.LoadScene("WIP");
        }


        public void QuitToTitle()
        {
            SceneManager.LoadScene(0);
        }


        public void SaveQuest(QuestJsonData questData)
        {
            SaveDataManager.Save(questData);
        }

        public void SaveQuest(QuestJsonData questData, List<Creature> adventurers, List<Creature> enemies)
        {
            questData.PartyMembers = SaveDataManager.SerializeCharacterData(adventurers);
            questData.Enemies = SaveDataManager.SerializeEnemyData(enemies);

            SaveDataManager.Save(questData);
        }


        public Encounter GetEncounter(int encounterID)
        {
            return _encounters.Where(w => w.ID == encounterID).Single();
        }


        public void LoadEncounter(int encounterID)
        {
            Encounter encounter = GetEncounter(encounterID);

            _questData = GetQuestData();

            _questData.SetNextEncounter(encounter);

        }

        #region Battle Encounters

        public void LostBattle()
        {
            // commit quest data to past runs data/high score leaderboard?
        }


        public void WonBattle()
        {
            //_questData = SaveDataManager.Load();
            //_questData.CurrentEncounterType = EncounterTypesEnum.Map;
            //GoToEncounterType(EncounterTypesEnum.Map);

            SceneManager.LoadScene("WIP");

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


        public NewBattleAction GetAction(int actionID)
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

        public CreatureModel GetCreatureModel(int creatureModelID)
        {
            return _characterModels.Where(w => w.ID == creatureModelID).Single();
        }


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
}