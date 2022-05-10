using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using JFlex.ChessQuesters.Core;
using JFlex.ChessQuesters.Core.ScriptableObjects;
using JFlex.ChessQuesters.Encounters.Battle.Grid;

namespace JFlex.ChessQuesters.Encounters.Battle
{
    public class BattleSystem : Singleton<BattleSystem>
    {

        private int _id;
        private int _layout;

        public BattleStatesEnum State;

        private GameManager _gameManager;

        //[SerializeField] private ActionManager _actionManager;

        //public GameGrid Grid;

        //[SerializeField] private GameObject HeroPrefab;
        //[SerializeField] private GameObject CharacterPrefab;

        private GameObject HeroObj;
        private GameObject CharacterObj;
        //private List<Adventurer> Adventurers;

        [SerializeField] private GameObject CreaturePanel;

        [SerializeField] private GameObject CameraRig;
        private CameraHandler CameraHandler;

        private Camera _camera;

        //[SerializeField] private GameObject _highlightMovePrefab;

        //[SerializeField] private GameObject _attackPrefab;
        //private GameObject _currentAttackTemplate;

        //[SerializeField]
        //private CinemachineVirtualCamera VirtualCamera;

        //[SerializeField]
        //private CinemachineFreeLook FreeLookCamera;

        private QuestJsonData _questData;


        private Dictionary<int, GameObject> _creaturePrefabs = new Dictionary<int, GameObject>();
        private Creature _activeCharacter;


        [SerializeField] private GameObject _characterPrefab;


        [SerializeField] private CreatureRuntimeSet _playerCharacters;
        [SerializeField] private CreatureRuntimeSet _enemies;
        [SerializeField] private CreatureRuntimeSet _targets;

        private EnemyActionResult _enemyAction;
        [SerializeField] private ActionResult _playerAction;

        [SerializeField] private IntVariable TurnNumber;
        [SerializeField] private IntVariable TurnPointer;

        private List<Creature> _affectedCreatures;



        public void Awake()
        {
            _camera = Camera.main;
            _affectedCreatures = new List<Creature>();


            State = BattleStatesEnum.START;

            CameraHandler = CameraRig.GetComponent<CameraHandler>();





            //BattleEvents.OnDeath += CharacterDied;

        }

        private void OnEnable()
        {
            //EventSystem.OnBattleStarted += BattleStarted;
            //BattleEvents.OnBattleVictory += GoToVictory;
            BattleEvents.OnTurnStart += StartNextTurn;


            //BattleEvents.OnCellMoveHighlighted += HighlightCell;
            //BattleEvents.OnCellMoveUnhighlighted += UnhighlightCell;
            BattleEvents.OnCellMoveSelected += SelectCell;
            //BattleEvents.OnCreatureMoved += SetupPlayerAttack;
            //BattleEvents.OnPlayerActionSelected += SetupAttackTemplate;
            //BattleEvents.OnCellAttackSelected += HandleAttack;

            //BattleEvents.OnCellAttackHighlighted += HighlightAttackCell;
            // BattleEvents.OnCellAttackUnhighlighted += UnhighlightAttackCell;
            //BattleEvents.OnCellUnhighlighted += UnhighlightCell;

            BattleEvents.OnCellSelected += SelectCell;

            BattleEvents.OnTakeDamageStart += AddToActionQueue;
            BattleEvents.OnTakeDamageFinish += RemoveFromActionQueue;

        }


        private void RemoveFromActionQueue(Creature creature)
        {
            if (_affectedCreatures.Contains(creature))
            {
                _affectedCreatures.Remove(creature);
            }
            if (_affectedCreatures.Count == 0)
            {
                State = BattleStatesEnum.PLAYER_IDLE;
            }
        }

        private void AddToActionQueue(Creature creature, int damage)
        {
            if (State != BattleStatesEnum.RESOLVING_PLAYER_ACTION)
            {
                State = BattleStatesEnum.RESOLVING_PLAYER_ACTION;
            }
            _affectedCreatures.Add(creature);
        }

        private void OnDisable()
        {
            //BattleEvents.OnBattleVictory -= GoToVictory;
            BattleEvents.OnTurnStart -= StartNextTurn;

            //BattleEvents.OnCellMoveHighlighted -= HighlightCell;
            //BattleEvents.OnCellMoveUnhighlighted -= UnhighlightCell;
            BattleEvents.OnCellMoveSelected -= SelectCell;
            //BattleEvents.OnCreatureMoved -= SetupPlayerAttack;
            //BattleEvents.OnPlayerActionSelected -= SetupAttackTemplate;
            //BattleEvents.OnCellAttackSelected -= HandleAttack;

            //BattleEvents.OnCellAttackHighlighted -= HighlightAttackCell;
            //BattleEvents.OnCellAttackUnhighlighted -= UnhighlightAttackCell;

            //BattleEvents.OnCellUnhighlighted -= UnhighlightCell;

            BattleEvents.OnCellSelected -= SelectCell;

            //BattleEvents.OnDeath -= CharacterDied;

            BattleEvents.OnTakeDamageStart -= AddToActionQueue;
            BattleEvents.OnTakeDamageFinish -= RemoveFromActionQueue;

            _playerCharacters.Empty();
            _enemies.Empty();

        }

        //private void CharacterDied(Creature creature)
        //{
        //    if (creature.IsFriendly)
        //    {
        //        //NewAdventurers = NewAdventurers.Where(w => w.ID != characterID).ToList();
        //        _playerCharacters.Remove(creature);
        //    }
        //    else
        //    {
        //        //NewEnemies = NewEnemies.Where(w => w.ID != characterID).ToList();
        //        _enemies.Remove(creature);
        //    }

        //    _combatants.Remove(creature);

        //}

        //private void HighlightAttackCell(GridCell cell)
        //{
        //    _currentAttackTemplate.transform.position = cell.transform.position + _highlightOffset;
        //    Debug.Log("Highlighting cell: (" + cell.X + ", " + cell.Y + ") for attack!");
        //    if (!_currentAttackTemplate.activeInHierarchy)
        //    {
        //        _currentAttackTemplate.SetActive(true);
        //    }
        //}



        //private void HandleAttack(GridCell cell)
        //{
        //    //if (_currentAttackTemplate != null)
        //    //{
        //    //    Destroy(_currentAttackTemplate);
        //    //}

        //    //StartCoroutine(PlayerAttackCoroutine(cell));
        //    StartCoroutine(PlayerActionCoroutine(cell));

        //}

        //public void 

        //private void SetupAttackTemplate(ActionClass action)
        //{
        //    _currentAction = action;

        //    if (_currentAttackTemplate != null)
        //    {
        //        Destroy(_currentAttackTemplate);
        //    }
        //    _currentAttackTemplate = Instantiate(action.AttackTemplatePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //    _currentAttackTemplate.SetActive(false);

        //}

        //private void HighlightCell(GridCell cell)
        //{

        //    _highlightMovePrefab.transform.position = cell.transform.position + _highlightOffset;
        //    //Debug.Log("Highlighting cell: (" + cell.X + ", " + cell.Y + ")");
        //    if (!_highlightMovePrefab.activeInHierarchy)
        //    {
        //        _highlightMovePrefab.SetActive(true);
        //    }



        //}


        //private void UnhighlightCell()
        //{
        //    if (State == BattleStatesEnum.PLAYER_MOVE)
        //    {
        //        _highlightMovePrefab.SetActive(false);
        //    }
        //    else if (State == BattleStatesEnum.PLAYER_ATTACK)
        //    {
        //        if (_currentAttackTemplate != null)
        //        {
        //            _currentAttackTemplate.SetActive(false);
        //        }
        //    }
        //}

        //private void UnhighlightAttackCell()
        //{
        //    if (_currentAttackTemplate != null)
        //    {
        //        _currentAttackTemplate.SetActive(false);
        //    }        
        //}


        private void SelectCell(GridCell cell)
        {

            GameGrid.Instance.ClearGrid();
            PlayerCharacter playerCharacter = (PlayerCharacter)_activeCharacter;

            playerCharacter.DoSelectedAction(_playerAction);

        }

        private void Start()
        {
            // load data here that needs to happen on scene load regardless of whether it's the player
            // starting the encounter, or resuming from save


            State = BattleStatesEnum.START;
            _questData = GameManager.Instance.GetQuestData();

            _id = _questData.Battle_ID;
            _layout = _questData.Battle_Layout;
            TurnNumber.SetValue(_questData.TurnNumber);
            TurnPointer.SetValue(_questData.TurnPointer);

            GameGrid.Instance.CreateGameGrid(10, 10);

            bool combatHasStarted = _questData.HasCombatStarted();

            if (!combatHasStarted)
            {
                // resume battle...            
                StartCoroutine(InitBattleCoroutine());
            }

            //IM.SetInitiative(_questData.Initiative);

            //if (!IM.HasCombatStarted())
            //{
            //    // resume battle...            
            //    StartCoroutine(InitBattleCoroutine());

            //}



            SpawnCharacters();
            SpawnEnemies();


            BattleEvents.FadeIn(() => StartCombat(combatHasStarted));

            //if (combatHasStarted)
            //{
            //    //StartNextTurn(_combatants.Items[_questData.TurnPointer].ID);
            //    BattleEvents.ResumeCombat();
            //}
            //else
            //{
            //    BattleEvents.StartCombat();
            //}

            //if (IM.HasCombatStarted())
            //{
            //    StartNextTurn(IM.ActiveCharacterID);
            //}
            //else
            //{
            //    BattleEvents.RollInitiative(Combatants);
            //}

        }


        private void StartCombat(bool combatHasStarted)
        {
            if (combatHasStarted)
            {
                //StartNextTurn(_combatants.Items[_questData.TurnPointer].ID);
                BattleEvents.ResumeCombat();
            }
            else
            {
                BattleEvents.StartCombat();
            }
        }


        private IEnumerator InitBattleCoroutine()
        {

            Debug.Log("Play opening animation");
            Encounter encounter = GameManager.Instance.GetEncounter(_questData.Battle_ID);

            int id = 1;
            for (int i = 0; i < _questData.PartyMembers.Length; i++)
            {
                Vector2 spawn = encounter.PlayerSpawns[i];

                CharacterJsonData adv = _questData.PartyMembers[i];
                adv.ID = id;

                GridCell cell = GameGrid.Instance.GetCell((int)spawn.x, (int)spawn.y);
                Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

                adv.SetPosition(cell.X, cell.Y, cellPos, 0);

                id++;
            }

            for (int i = 0; i < _questData.Enemies.Length; i++)
            {
                Vector2 spawn = encounter.EnemySpawns[i];
                GridCell cell = GameGrid.Instance.GetCell((int)spawn.x, (int)spawn.y);
                Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

                EnemyJsonData adv = _questData.Enemies[i];
                adv.ID = id;

                adv.SetPosition(cell.X, cell.Y, cellPos, 2);
                id++;
            }

            yield return null;
        }


        private void SpawnCharacters()
        {

            // set character positions
            foreach (CharacterJsonData c in _questData.PartyMembers)
            {
                if (c.Health <= 0)
                    continue;

                GridCell cell = GameGrid.Instance.GetCell(c.CellX, c.CellY);
                Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

                Quaternion rot = GetCharacterRotation(c.CurrentFacing);

                //if (_creaturePrefabs.ContainsKey(c.CreatureModelID))
                //{
                //    currentPrefab = _creaturePrefabs[c.CreatureModelID];
                //}
                //else
                //{
                //    currentPrefab = GameManager.Instance.GetCreatureModelPrefab(c.CreatureModelID);
                //    _creaturePrefabs.Add(c.CreatureModelID, currentPrefab);
                //}

                CharacterObj = Instantiate(_characterPrefab, cellPos, Quaternion.identity * rot);

                CharacterObj.name = c.Name;
                PlayerCharacter ic = CharacterObj.GetComponent<PlayerCharacter>();
                ic.InitFromCharacterData(c);

                _playerCharacters.Add(ic);

            }
        }


        private void SpawnEnemies()
        {

            GameObject currentPrefab = null;

            // init enemies...
            foreach (EnemyJsonData c in _questData.Enemies)
            {

                if (c.Health <= 0)
                    continue;

                EnemySO enemyObject = GameManager.Instance.GetEnemyObject(c.EnemyID);

                GridCell cell = GameGrid.Instance.GetCell(c.CellX, c.CellY);
                Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

                Quaternion rot = GetCharacterRotation(c.CurrentFacing);

                if (_creaturePrefabs.ContainsKey(enemyObject.CreatureModelID))
                {
                    currentPrefab = _creaturePrefabs[enemyObject.CreatureModelID];
                }
                else
                {

                    currentPrefab = enemyObject.ModelPrefab;
                    _creaturePrefabs.Add(enemyObject.CreatureModelID, currentPrefab);
                }

                CharacterObj = Instantiate(currentPrefab, cellPos, Quaternion.identity * rot);
                CharacterObj.name = c.Name;
                Enemy enemy = CharacterObj.GetComponent<Enemy>();
                enemy.Init(c, enemyObject);

                _enemies.Add(enemy);

            }

        }


        private Quaternion GetCharacterRotation(int facing)
        {
            Quaternion rot = Quaternion.Euler(0, 0, 0);

            switch (facing)
            {
                case 0:
                    rot = Quaternion.Euler(0, 0, 0);
                    break;
                case 1:
                    rot = Quaternion.Euler(0, 90, 0);
                    break;
                case 2:
                    rot = Quaternion.Euler(0, 180, 0);
                    break;
                case 3:
                    rot = Quaternion.Euler(0, -90, 0);
                    break;
                default:
                    Debug.Log("Facing " + facing + " not recognised!");
                    break;
            }

            return rot;
        }


        public void SaveProgress()
        {
            _questData.TurnNumber = TurnNumber.Value;
            _questData.TurnPointer = TurnPointer.Value;

            GameManager.Instance.SaveQuest(_questData, _playerCharacters.Items, _enemies.Items);
        }


        private void StartNextTurn(Creature activeCharacter)
        {

            SaveProgress();
            _activeCharacter = activeCharacter;

            State = BattleStatesEnum.START_TURN;

            CameraHandler.SwapToCharacter(_activeCharacter.Position);

            if (_activeCharacter.IsFriendly)
            {
                BattleEvents.StartPlayerTurn();
                State = BattleStatesEnum.PLAYER_IDLE;

            }
            else
            {
                BattleEvents.EndPlayerTurn();
                State = BattleStatesEnum.ENEMY_IDLE;
                StartCoroutine(EnemyActionCoroutine());
            }

        }


        //IEnumerator PlayerActionCoroutine(GridCell cell)
        //{
        //    GameGrid.Instance.ClearGrid();

        //    Debug.Log("PLAYER ACTION!");

        //    NewBattleAction action = _playerAction.Action;


        //    _activeCharacter.DoAction(_playerAction);

        //    action.DoAction();
            
        //    BattleEvents.ActionPerformed(_playerAction.Action);

        //    yield break;
        //}


        private IEnumerator EnemyActionCoroutine()
        {
            yield return new WaitForSeconds(2f);

            Enemy enemy = (Enemy)_activeCharacter;


            enemy.CalcActions();
        }


        public void Update()
        {

            if (_activeCharacter != null && _activeCharacter.State == CharacterStatesEnum.MOVING)
            {
                CameraHandler.LookAtCreature(_activeCharacter.Transform);
            }

        }

        public void TestButton()
        {
            BattleEvents.Victory();
        }

    }




    public enum BattleStatesEnum
    {
        START,
        START_TURN,
        PLAYER_IDLE,
        RESOLVING_PLAYER_ACTION,
        ENEMY_IDLE,
        RESOLVING_ENEMY_ACTION
    }
}