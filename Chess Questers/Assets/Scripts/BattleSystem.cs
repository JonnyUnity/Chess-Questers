using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

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

    private BattleUIHandler UIHandler;

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
    private List<PlayerCharacter> NewAdventurers = new List<PlayerCharacter>();
    private List<Enemy> NewEnemies = new List<Enemy>();
    private List<Creature> Combatants = new List<Creature>();

    private ActionClass _currentAction;

    private Dictionary<int, GameObject> _creaturePrefabs = new Dictionary<int, GameObject>();
    private Creature _activeCharacter;

    private readonly Vector3 _highlightOffset = new Vector3(0, 0.01f, 0);

    [SerializeField] private GameObject _characterPrefab;

    [SerializeField] private GameObject _gridLines;

    private Dictionary<int, Vector2> _creaturePositions = new Dictionary<int, Vector2>();

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
        UIHandler = CreaturePanel.GetComponent<BattleUIHandler>();

        CameraHandler = CameraRig.GetComponent<CameraHandler>();

        

        //EventSystem.OnBattleStarted += BattleStarted;
        //BattleEvents.OnBattleVictory += GoToVictory;
        BattleEvents.OnTurnStart += StartNextTurn;


        //BattleEvents.OnCellMoveHighlighted += HighlightCell;
        //BattleEvents.OnCellMoveUnhighlighted += UnhighlightCell;
        BattleEvents.OnCellMoveSelected += SelectCell;
        //BattleEvents.OnCreatureMoved += SetupPlayerAttack;
        //BattleEvents.OnPlayerActionSelected += SetupAttackTemplate;
        BattleEvents.OnCellAttackSelected += HandleAttack;

        //BattleEvents.OnCellAttackHighlighted += HighlightAttackCell;
        // BattleEvents.OnCellAttackUnhighlighted += UnhighlightAttackCell;
        //BattleEvents.OnCellUnhighlighted += UnhighlightCell;

        BattleEvents.OnCellSelected += SelectCell;

        BattleEvents.OnTakeDamageStart += AddToActionQueue;
        BattleEvents.OnTakeDamageFinish += RemoveFromActionQueue;

        //BattleEvents.OnDeath += CharacterDied;

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
        BattleEvents.OnCellAttackSelected -= HandleAttack;

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



    private void HandleAttack(GridCell cell)
    {
        //if (_currentAttackTemplate != null)
        //{
        //    Destroy(_currentAttackTemplate);
        //}

        //StartCoroutine(PlayerAttackCoroutine(cell));
        StartCoroutine(PlayerActionCoroutine(cell));

    }

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

        //StartCoroutine(PlayerActionCoroutine(cell));

        GameGrid.Instance.ClearGrid();

        UIHandler.UpdateStateText("PLAYER ACTION");

        //Debug.Log("PLAYER ACTION!");

        //NewBattleAction action = _playerAction.Action;

        PlayerCharacter playerCharacter = (PlayerCharacter)_activeCharacter;

        playerCharacter.DoSelectedAction(_playerAction);

        //_activeCharacter.DoAction(_playerAction);

        //if (action.IsMove)
        //{
        //    Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);
        //    _activeCharacter.DoMove(cellPos, cell.X, cell.Y);


        //    do
        //    {
        //        yield return null;
        //    } while (_activeCharacter.State == CharacterStatesEnum.MOVING);

        //    _activeCharacter.UpdatePosition(cell.X, cell.Y, cellPos, _activeCharacter.CurrentFacing);

        //    //_activeCharacter.MoveAction.DoAction();
        //    //BattleEvents.CreatureMoved(_activeCharacter);
        //    //BattleEvents.ActionPerformed(_activeCharacter.MoveAction);

        //}
        //else
        //{

        //    UIHandler.UpdateStateText("PLAYER ATTACK");

        //    Debug.Log("PLAYER ATTACK!");

        //    //Creature adv = InitiativeManager.GetCurrentCreature();
        //    //PlayerCharacter pc = (PlayerCharacter)_activeCharacter;

        //    //int damage = pc.GetAttackDamage();
        //    //int damage = _currentAction.Damage;
        //    int damage = _playerAction.Action.Damage;

        //    foreach (var creature in _playerAction.Creatures)
        //    {
        //        BattleEvents.TakeDamage(creature, damage);
        //    }



        //}
        //action.DoAction();

        //BattleEvents.ActionPerformed(_playerAction.Action);



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

        GameGrid.Instance.CreateGameGrid();

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
        
        if (combatHasStarted)
        {
            //StartNextTurn(_combatants.Items[_questData.TurnPointer].ID);
            BattleEvents.ResumeCombat();
        }
        else
        {
            BattleEvents.StartCombat();
        }

        //if (IM.HasCombatStarted())
        //{
        //    StartNextTurn(IM.ActiveCharacterID);
        //}
        //else
        //{
        //    BattleEvents.RollInitiative(Combatants);
        //}

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

            NewEnemyJsonData adv = _questData.Enemies[i];
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


            NewAdventurers.Add(ic);


            ic.OccupiedCell = cell;

            GameGrid.Instance.AddCreaturePosition(ic);
        }



    }

    private void SpawnEnemies()
    {

        GameObject currentPrefab = null;

        // init enemies...
        foreach (NewEnemyJsonData c in _questData.Enemies)
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

            NewEnemies.Add(enemy);
            _enemies.Add(enemy);

            enemy.OccupiedCell = cell;

            GameGrid.Instance.AddCreaturePosition(enemy);
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

        GameManager.Instance.SaveQuestNew(_questData, _playerCharacters.Items, _enemies.Items);
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





    //private IEnumerator SetUpPlayerMoveCoroutine(Creature creature)
    //{
    //    yield return new WaitForSeconds(1f);

    //    UIHandler.UpdateStateText("CALC PLAYER MOVES");
    //    //GameGrid.Instance.ShowMovesForPlayer(creature.MoveAction, creature.CellX, creature.CellY);
    //    //GameGrid.Instance.ShowMovesForPlayerNew(creature.MoveClass, creature.CellX, creature.CellY);
    //    UIHandler.UpdateStateText("WAITING FOR PLAYER MOVE");

    //}

    //IEnumerator PlayerMoveCoroutine(GridCell cell)
    //{
    //    GameGrid.Instance.ClearGrid();
    //    GameGrid.Instance.HideGrid();

    //    UIHandler.UpdateStateText("PLAYER MOVE");

    //    Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

    //    _activeCharacter.OccupiedCell.ResetCell();

    //    _activeCharacter.DoMove(cellPos, cell.X, cell.Y);

    //    do
    //    {
    //        yield return null;
    //    } while (_activeCharacter.State == CharacterStatesEnum.MOVING);

    //    //cell.SetUnit(_activeCharacter);
    //    _activeCharacter.UpdatePosition(cell.X, cell.Y, cellPos, _activeCharacter.CurrentFacing);

    //    _activeCharacter.MoveAction.DoAction();
    //    //BattleEvents.CreatureMoved(_activeCharacter);
    //    BattleEvents.ActionPerformed(_activeCharacter.MoveAction);

    //    //yield return new WaitForSeconds(0.1f);

    //    //State = BattleStatesEnum.PLAYER_ATTACK;

    //    //UIHandler.BuildAttackList(_activeCharacter);

    //    // debug
    //    yield return null;
    //}


    //private void SetupPlayerAttack(Creature creature)
    //{

    //    if (!creature.IsFriendly)
    //        return;

    //    UIHandler.UpdateStateText("CALC PLAYER ATTACKS");

    //    //var actions = _activeCharacter.AvailableActions;

    //    //State = BattleStatesEnum.PLAYER_ATTACK;
    //    State = BattleStatesEnum.PLAYER_ACTION;

    //    //Creature adv = InitiativeManager.GetCurrentCreature();
    //    //adv.SetSelectedAttack(0);



    //    //var attacks = Grid.GetAttacksForPlayer(adv.X, adv.Y);
    //    //var attacks = Grid.GetBaseAttack(adv.OccupiedCell);
    //    //if (attacks.Count == 0)
    //    //{
    //    //    // only show pass option...
    //    //}

    //    // SetupAttackUI...

    //    //_currentAttackTemplate = Instantiate(_attackPrefab, new Vector3(0, 0, 0), Quaternion.identity);

    //    //_actionManager.SetActions();

    //   // UIHandler.ShowActions(_activeCharacter, _activeCharacter.Actions, _activeCharacter.CellX, _activeCharacter.CellY);

    //    //UIHandler.UpdateCharacterText(adv.CreatureName);
    //    UIHandler.UpdateStateText("WAITING FOR PLAYER ACTION");
    //}


    IEnumerator PlayerActionCoroutine(GridCell cell)
    {
        GameGrid.Instance.ClearGrid();

        UIHandler.UpdateStateText("PLAYER ACTION");

        Debug.Log("PLAYER ACTION!");

        NewBattleAction action = _playerAction.Action;


        _activeCharacter.DoAction(_playerAction);
        
        //if (action.IsMove)
        //{
        //    Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);
        //    _activeCharacter.DoMove(cellPos, cell.X, cell.Y);


        //    do
        //    {
        //        yield return null;
        //    } while (_activeCharacter.State == CharacterStatesEnum.MOVING);

        //    _activeCharacter.UpdatePosition(cell.X, cell.Y, cellPos, _activeCharacter.CurrentFacing);

        //    //_activeCharacter.MoveAction.DoAction();
        //    //BattleEvents.CreatureMoved(_activeCharacter);
        //    //BattleEvents.ActionPerformed(_activeCharacter.MoveAction);

        //}
        //else
        //{

        //    UIHandler.UpdateStateText("PLAYER ATTACK");

        //    Debug.Log("PLAYER ATTACK!");

        //    //Creature adv = InitiativeManager.GetCurrentCreature();
        //    //PlayerCharacter pc = (PlayerCharacter)_activeCharacter;

        //    //int damage = pc.GetAttackDamage();
        //    //int damage = _currentAction.Damage;
        //    int damage = _playerAction.Action.Damage;

        //    foreach (var creature in _playerAction.Creatures)
        //    {
        //        BattleEvents.TakeDamage(creature, damage);
        //    }



        //}
        action.DoAction();
        yield return null;

        BattleEvents.ActionPerformed(_playerAction.Action);

    }

    //IEnumerator PlayerAttackCoroutine(GridCell cell)
    //{
    //    GameGrid.Instance.ClearGrid();

    //    // do attack!
    //    UIHandler.UpdateStateText("PLAYER ATTACK");

    //    Debug.Log("PLAYER ATTACK!");

    //    //Creature adv = InitiativeManager.GetCurrentCreature();
    //    //PlayerCharacter pc = (PlayerCharacter)_activeCharacter;

    //    //int damage = pc.GetAttackDamage();
    //    //int damage = _currentAction.Damage;
    //    int damage = _playerAction.Action.Damage;

    //    foreach (var creature in _playerAction.Creatures)
    //    {
    //        BattleEvents.TakeDamage(creature, damage);
    //    }

    //    //_playerAction.Action.DoAction();     //rename this!

    //    // update attacked creatures list to creatures, not ids!
    //    //List<int> attackedCreatures = GameGrid.Instance.GetAttackedCreatures(cell, _currentAction);

    //    //foreach (int creatureID in attackedCreatures)
    //    //{
    //    //    BattleEvents.TakeDamage(pc, damage);
    //    //}

    //    //// new method
    //    //foreach (Creature creature in _targets.Items)
    //    //{
    //    //    BattleEvents.TakeDamage(creature, damage);
    //    //}
    //    _targets.Empty();

    //    yield return new WaitForSeconds(1f);

    //    //BattleEvents.TurnOver();
    //    BattleEvents.ActionPerformed(_playerAction.Action);

    //}




    private IEnumerator EnemyActionCoroutine()
    {
        UIHandler.UpdateStateText("ENEMY ACTION");
        yield return new WaitForSeconds(2f);

        Enemy enemy = (Enemy)_activeCharacter;


        enemy.CalcActions();

        //enemy.ActionsRemaining = enemy.ActionsPerTurn;

        //while (enemy.ActionsRemaining > 0)
        //{
        //    UIHandler.UpdateStateText("THINKING!");
        //    //Debug.Log("THINKING!");
        //    yield return new WaitForSeconds(3f);
        //    var enemyAction = enemy.CalcAction();
        //    if (enemyAction == null)
        //    {
        //        //BattleEvents.TurnOver();
        //        //enemy.ActionsRemaining = 0;
        //        //BattleEvents.ActionFinished();
        //        UIHandler.UpdateStateText("ENEMY SKIPPING!");
        //        break;
        //    }    
        //    else
        //    {
        //        UIHandler.UpdateStateText(enemyAction.Action.Name);
        //    }

        //    //if (enemy.CalcAction() == null)
        //    //{
        //    //    // No valid actions found so pass turn!
        //    //    BattleEvents.TurnOver();
        //    //    break;
        //    //}
            
        //}


        //yield return new WaitForSeconds(1f);

        //UIHandler.UpdateStateText("ENEMY TURN OVER");

        //BattleEvents.TurnOver();
        ////BattleEvents.action
        //GameGrid.Instance.ClearGrid();

    }

    //private IEnumerator EnemyMoveCoroutine()
    //{

    //    UIHandler.UpdateStateText("ENEMY MOVE");

    //    yield return new WaitForSeconds(2f);

    //    // calculate move...
        

    //    Enemy thisEnemy = (Enemy)_activeCharacter;

    //    thisEnemy.CalcMove();

    //    do
    //    {
    //        yield return null;
    //    } while (_activeCharacter.State == CharacterStatesEnum.MOVING);

    //    //cell.SetUnit(_activeCharacter);
    //    //_activeCharacter.UpdatePosition(cell.X, cell.Y, cellPos, _activeCharacter.CurrentFacing);

    //    BattleEvents.CreatureMoved(_activeCharacter);

    //    yield return new WaitForSeconds(2f);



    //   //BattleEvents.CreatureMoved(_activeCharacter);

    //    State = BattleStatesEnum.ENEMY_ATTACK;
    //    StartCoroutine(EnemyAttackCoroutine());

    //}


    //private IEnumerator EnemyAttackCoroutine()
    //{
    //    // do attack
    //    UIHandler.UpdateStateText("ENEMY ATTACK");

    //    yield return new WaitForSeconds(1f);

    //    Enemy thisEnemy = (Enemy)_activeCharacter;
    //    _targets.Empty();

    //    // Calc Attack will populate the enemy Action object with the selected cell and the affected creatures
    //    _enemyAction = thisEnemy.CalcAttack();

    //    if (_enemyAction != null)
    //    {
    //        UIHandler.UpdateStateText(_enemyAction.Action.Name);

    //        foreach (Creature creature in _enemyAction.Creatures)
    //        {
    //            BattleEvents.TakeDamage(creature, _enemyAction.Damage);
    //        }

    //        _enemyAction.Action.DoAction();

    //    }
    //    else
    //    {
    //        // no action so skip!
    //        UIHandler.UpdateStateText("NO VALID TARGET!");
    //    }


    //    //if (enemyAction.TargetCell != null)
    //    //{
    //    //    // Change attacked craeatures to be list of creatures, not IDs
    //    //    List<int> attackedCreatures = GameGrid.Instance.GetAttackedCreatures(enemyAction.TargetCell, enemyAction.Action);

    //    //    foreach (int creatureID in attackedCreatures)
    //    //    {

                
    //    //        BattleEvents.TakeDamage(thisEnemy, enemyAction.Action.Damage);
    //    //    }

    //    //    // new method!
    //    //    foreach (Creature creature in _targets.Items)
    //    //    {
    //    //        BattleEvents.TakeDamage(creature, enemyAction.Action.Damage);
    //    //    }
    //    //    _targets.Empty();

    //    //}
    //    //else
    //    //{
    //    //    UIHandler.UpdateStateText("NO VALID TARGET!");
    //    //}

    //    yield return new WaitForSeconds(1f);

    //    BattleEvents.TurnOver();
    //    GameGrid.Instance.ClearGrid();

    //}


    public void Update()
    {

        if (_activeCharacter != null && _activeCharacter.State == CharacterStatesEnum.MOVING)
        {
            CameraHandler.LookAtCreature(_activeCharacter.Transform);
        }

    }


    private void GoToVictory()
    {
        Debug.Log("EVENT: VICTORY!");
        SceneManager.LoadSceneAsync(3);
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