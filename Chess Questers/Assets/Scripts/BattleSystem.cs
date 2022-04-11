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
    private InitiativeManager InitiativeManager;
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

    [SerializeField] private GameObject _highlightMovePrefab;

    [SerializeField] private GameObject _attackPrefab;
    private GameObject _currentAttackTemplate;

    //[SerializeField]
    //private CinemachineVirtualCamera VirtualCamera;

    //[SerializeField]
    //private CinemachineFreeLook FreeLookCamera;

    private QuestJsonData _questData;
    private List<PlayerCharacter> NewAdventurers = new List<PlayerCharacter>();
    private List<Enemy> NewEnemies = new List<Enemy>();
    private List<Creature> Combatants = new List<Creature>();
    private Creature _activeCharacter;

    private ActionClass _currentAction;

    [SerializeField] private NewIM IM;

    private Dictionary<int, GameObject> _creaturePrefabs = new Dictionary<int, GameObject>();


    private readonly Vector3 _highlightOffset = new Vector3(0, 0.01f, 0);


    [SerializeField] private GameObject _gridLines;

    private Dictionary<int, Vector2> _creaturePositions = new Dictionary<int, Vector2>();

    public void Awake()
    {
        _camera = Camera.main;

        State = BattleStatesEnum.START;
        UIHandler = CreaturePanel.GetComponent<BattleUIHandler>();

        CameraHandler = CameraRig.GetComponent<CameraHandler>();

        InitiativeManager = GetComponent<InitiativeManager>();

        //EventSystem.OnBattleStarted += BattleStarted;
        BattleEvents.OnBattleVictory += GoToVictory;
        BattleEvents.OnTurnStart += StartNextTurn;


        BattleEvents.OnCellMoveHighlighted += HighlightCell;
        BattleEvents.OnCellMoveUnhighlighted += UnhighlightCell;
        BattleEvents.OnCellMoveSelected += SelectCell;
        BattleEvents.OnCreatureMoved += SetupPlayerAttack;
        BattleEvents.OnPlayerActionSelected += SetupAttackTemplate;
        BattleEvents.OnCellAttackSelected += HandleAttack;

        BattleEvents.OnCellAttackHighlighted += HighlightAttackCell;
        BattleEvents.OnCellAttackUnhighlighted += UnhighlightAttackCell;

        BattleEvents.OnDeath += CharacterDied;

    }



    private void OnDisable()
    {
        BattleEvents.OnBattleVictory -= GoToVictory;
        BattleEvents.OnTurnStart -= StartNextTurn;

        BattleEvents.OnCellMoveHighlighted -= HighlightCell;
        BattleEvents.OnCellMoveUnhighlighted -= UnhighlightCell;
        BattleEvents.OnCellMoveSelected -= SelectCell;
        BattleEvents.OnCreatureMoved -= SetupPlayerAttack;
        BattleEvents.OnPlayerActionSelected -= SetupAttackTemplate;
        BattleEvents.OnCellAttackSelected -= HandleAttack;

        BattleEvents.OnCellAttackHighlighted -= HighlightAttackCell;
        BattleEvents.OnCellAttackUnhighlighted -= UnhighlightAttackCell;
        
        BattleEvents.OnDeath -= CharacterDied;

    }

    private void CharacterDied(int characterID, bool isFriendly)
    {
        if (isFriendly)
        {
            NewAdventurers = NewAdventurers.Where(w => w.ID != characterID).ToList();
        }
        else
        {
            NewEnemies = NewEnemies.Where(w => w.ID != characterID).ToList();
        }
    }

    private void HighlightAttackCell(GridCell cell)
    {
        _currentAttackTemplate.transform.position = cell.transform.position + _highlightOffset;
        Debug.Log("Highlighting cell: (" + cell.X + ", " + cell.Y + ") for attack!");
        if (!_currentAttackTemplate.activeInHierarchy)
        {
            _currentAttackTemplate.SetActive(true);
        }
    }



    private void HandleAttack(GridCell cell)
    {
        if (_currentAttackTemplate != null)
        {
            Destroy(_currentAttackTemplate);
        }

        StartCoroutine(PlayerAttackCoroutine(cell));

    }

    private void SetupAttackTemplate(int characterID, ActionClass action, int x, int y)
    {
        _currentAction = action;

        if (_currentAttackTemplate != null)
        {
            Destroy(_currentAttackTemplate);
        }
        _currentAttackTemplate = Instantiate(action.AttackTemplatePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
    }

    private void HighlightCell(GridCell cell)
    {

        _highlightMovePrefab.transform.position = cell.transform.position + _highlightOffset;
        Debug.Log("Highlighting cell: (" + cell.X + ", " + cell.Y + ")");
        if (!_highlightMovePrefab.activeInHierarchy)
        {
            _highlightMovePrefab.SetActive(true);
        }


        
    }


    private void UnhighlightCell()
    {
        _highlightMovePrefab.SetActive(false);
    }

    private void UnhighlightAttackCell()
    {
        if (_currentAttackTemplate != null)
        {
            _currentAttackTemplate.SetActive(false);
        }        
    }


    private void SelectCell(GridCell cell)
    {
        _highlightMovePrefab.SetActive(false);
        StartCoroutine(PlayerMoveCoroutine(cell));
    }


    private void Start()
    {
        // load data here that needs to happen on scene load regardless of whether it's the player
        // starting the encounter, or resuming from save

        State = BattleStatesEnum.START;
        _questData = GameManager.Instance.GetQuestData();

        _id = _questData.Battle_ID;
        _layout = _questData.Battle_Layout;

        GameGrid.Instance.CreateGameGrid(this);

        IM.SetInitiative(_questData.Initiative);
        
        if (!IM.HasCombatStarted())
        {
            // resume battle...            
            StartCoroutine(InitBattleCoroutine());

        }

        SpawnCharacters();
        SpawnEnemies();
        
        if (IM.HasCombatStarted())
        {
            StartNextTurn(IM.ActiveCharacterID);
        }
        else
        {
            BattleEvents.RollInitiative(Combatants);
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

            //ImprovedCharacter adv = NewAdventurers[i];
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

            //ImprovedCharacter adv = NewEnemies[i];
            //CharacterJsonData adv = _questData.Enemies[i];
            NewEnemyJsonData adv = _questData.Enemies[i];
            adv.ID = id;

            adv.SetPosition(cell.X, cell.Y, cellPos, 2);
            id++;
        }

        //EventSystem.RollInitiative(Combatants);


        yield return null;
    }

    private void SpawnCharacters()
    {

        GameObject currentPrefab = null;

        // set character positions
        foreach (CharacterJsonData c in _questData.PartyMembers)
        {
            GridCell cell = GameGrid.Instance.GetCell(c.CellX, c.CellY);
            Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

            Quaternion rot = GetCharacterRotation(c.CurrentFacing);

            if (_creaturePrefabs.ContainsKey(c.CharacterModel))
            {
                currentPrefab = _creaturePrefabs[c.CharacterModel];
            }
            else
            {
                currentPrefab = GameManager.Instance.GetCreatureModelPrefab(c.CharacterModel);
                _creaturePrefabs.Add(c.CharacterModel, currentPrefab);
            }

            CharacterObj = Instantiate(currentPrefab, cellPos, Quaternion.identity * rot);
            CharacterObj.name = c.Name;
            PlayerCharacter ic = CharacterObj.GetComponent<PlayerCharacter>();
            ic.InitFromCharacterData(c);

            NewAdventurers.Add(ic);
            Combatants.Add(ic);

            ic.OccupiedCell = cell;
            //cell.SetUnit(ic);

            //_creaturePositions.Add(ic.ID, new Vector2(c.CellX, c.CellY));
            GameGrid.Instance.AddCreaturePosition(ic);
        }



    }

    private void SpawnEnemies()
    {

        GameObject currentPrefab = null;

        // init enemies...
        foreach (NewEnemyJsonData c in _questData.Enemies)
        {

            EnemySO enemyObject = GameManager.Instance.GetEnemyObject(c.EnemyID);

            GridCell cell = GameGrid.Instance.GetCell(c.CellX, c.CellY);
            Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

            Quaternion rot = GetCharacterRotation(c.CurrentFacing);

            if (_creaturePrefabs.ContainsKey(enemyObject.CharacterModel))
            {
                currentPrefab = _creaturePrefabs[enemyObject.CharacterModel];
            }
            else
            {
                currentPrefab = GameManager.Instance.GetCreatureModelPrefab(enemyObject.CharacterModel);
                _creaturePrefabs.Add(enemyObject.CharacterModel, currentPrefab);
            }

            CharacterObj = Instantiate(currentPrefab, cellPos, Quaternion.identity * rot);
            CharacterObj.name = c.Name;
            Enemy enemy = CharacterObj.GetComponent<Enemy>();
            enemy.Init(c, enemyObject);

            //ImprovedCharacter ic = CharacterObj.GetComponent<ImprovedCharacter>();
            //ic.InitFromEnemyData(c);

            NewEnemies.Add(enemy);
            Combatants.Add(enemy);

            enemy.OccupiedCell = cell;
            //cell.SetUnit(enemy);

            //_creaturePositions.Add(enemy.ID, new Vector2(c.CellX, c.CellY));
            GameGrid.Instance.AddCreaturePosition(enemy);
        }

    }


    private Quaternion GetCharacterRotation(int facing)
    {
        Quaternion rot = Quaternion.Euler(0, -90, 0);

        switch (facing)
        {
            case 0:
                rot = Quaternion.Euler(0, -90, 0);
                break;
            case 1:
                rot = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                rot = Quaternion.Euler(0, 90, 0);
                break;
            case 3:
                rot = Quaternion.Euler(0, 180, 0);
                break;
            default:
                Debug.Log("Facing " + facing + " not recognised!");
                break;
        }

        return rot;
    }


    public void SaveQuest()
    {
        StartCoroutine(SaveProgress());

    }

    public IEnumerator SaveProgress()
    {

        _questData.Initiative = IM._init; // change this? looks horrible
        
        GameManager.Instance.SaveQuest(_questData, NewAdventurers, NewEnemies);

        yield return null;
    }

    private void StartNextTurn(int characterID)
    {

        StartCoroutine(SaveProgress()); // perhaps move to end of turn?

        Debug.Log("BattleSystem - Start Next Turn! " + characterID);
        State = BattleStatesEnum.START_TURN;

        _activeCharacter = Combatants.Where(w => w.ID == characterID).Single();

        CameraHandler.SwapToCharacter(_activeCharacter.Position);

        UIHandler.UpdateCharacterText(_activeCharacter.Name);

        if (_activeCharacter.IsFriendly)
        {
            _gridLines.SetActive(true);
            State = BattleStatesEnum.PLAYER_MOVE;
            //SetupPlayerMove(_activeCharacter);
            StartCoroutine(SetUpPlayerMoveCoroutine(_activeCharacter));

        }
        else
        {
            _gridLines.SetActive(false);
            State = BattleStatesEnum.ENEMY_MOVE;
            StartCoroutine(EnemyMoveCoroutine());
        }

    }



    //public void OnGridCellClick(GridCell cell)
    //{
    //    if (State == BattleStatesEnum.PLAYER_MOVE && cell.IsMove)
    //    {
    //        StartCoroutine(PlayerMoveCoroutine(cell.X, cell.Y));
    //    }
    //    else if (State == BattleStatesEnum.PLAYER_ATTACK && cell.IsAttack)
    //    {
    //        StartCoroutine(PlayerAttackCoroutine(cell.X, cell.Y));
    //    }
    //}

    void SetupPlayerMove(Creature c)
    {
        UIHandler.UpdateStateText("CALC PLAYER MOVES");
        //Creature adv = InitiativeManager.GetCurrentCreature();

        //Grid.GetMovesForPlayer(adv.MoveClass, adv.X, adv.Y);
        //Grid.GetMovesForPlayer(c.MoveClass, c.CellX, c.CellY);

        GameGrid.Instance.ShowMovesForPlayer(c.MoveClass, c.CellX, c.CellY);

        UIHandler.UpdateStateText("WAITING FOR PLAYER MOVE");
        
    }


    private IEnumerator SetUpPlayerMoveCoroutine(Creature creature)
    {
        yield return new WaitForSeconds(1f);

        UIHandler.UpdateStateText("CALC PLAYER MOVES");
        GameGrid.Instance.ShowMovesForPlayer(creature.MoveClass, creature.CellX, creature.CellY);
        UIHandler.UpdateStateText("WAITING FOR PLAYER MOVE");

    }

    public void CharacterLook(int x, int y)
    {
        //Creature adv = InitiativeManager.GetCurrentCreature();
        //GridCell cell = Grid.GetCell(x, y);
        //Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);
        //adv.LookAtCell(cellPos);


    }

    IEnumerator PlayerMoveCoroutine(GridCell cell)
    {
        GameGrid.Instance.ClearGrid();

        UIHandler.UpdateStateText("PLAYER MOVE");

        Vector3 cellPos = GameGrid.Instance.GetGridCellWorldPosition(cell);

        //Creature adv = InitiativeManager.GetCurrentCreature();
        _activeCharacter.OccupiedCell.ResetCell();

        _activeCharacter.DoMove(cellPos, cell.X, cell.Y);

        do
        {
            yield return null;
        } while (_activeCharacter.State == CharacterStatesEnum.MOVING);

        //cell.SetUnit(_activeCharacter);
        _activeCharacter.UpdatePosition(cell.X, cell.Y, cellPos, _activeCharacter.CurrentFacing);
        
        BattleEvents.CreatureMoved(_activeCharacter);

        //yield return new WaitForSeconds(0.1f);

        //State = BattleStatesEnum.PLAYER_ATTACK;

        //UIHandler.BuildAttackList(_activeCharacter);

        // debug
        yield return null;
    }


    private void SetupPlayerAttack(Creature creature)
    {

        if (!creature.IsFriendly)
            return;

        UIHandler.UpdateStateText("CALC PLAYER ATTACKS");

        var actions = _activeCharacter.Actions;

        State = BattleStatesEnum.PLAYER_ATTACK;

        //Creature adv = InitiativeManager.GetCurrentCreature();
        //adv.SetSelectedAttack(0);



        //var attacks = Grid.GetAttacksForPlayer(adv.X, adv.Y);
        //var attacks = Grid.GetBaseAttack(adv.OccupiedCell);
        //if (attacks.Count == 0)
        //{
        //    // only show pass option...
        //}

        // SetupAttackUI...

        //_currentAttackTemplate = Instantiate(_attackPrefab, new Vector3(0, 0, 0), Quaternion.identity);


        UIHandler.ShowActions(_activeCharacter.ID, actions, _activeCharacter.CellX, _activeCharacter.CellY);

        //UIHandler.UpdateCharacterText(adv.CreatureName);
        UIHandler.UpdateCharacterText(_activeCharacter.Name);
        UIHandler.UpdateStateText("WAITING FOR PLAYER ATTACK");
    }


    IEnumerator PlayerAttackCoroutine(GridCell cell)
    {
        GameGrid.Instance.ClearGrid();

        // do attack!
        UIHandler.UpdateStateText("PLAYER ATTACK");

        Debug.Log("PLAYER ATTACK!");

        //Creature adv = InitiativeManager.GetCurrentCreature();
        PlayerCharacter pc = (PlayerCharacter)_activeCharacter;

        int damage = pc.GetAttackDamage();
        List<int> attackedCreatures = GameGrid.Instance.GetAttackedCreatures(cell, _currentAction);

        foreach (int creatureID in attackedCreatures)
        {
            BattleEvents.TakeDamage(creatureID, damage);
        }

        yield return new WaitForSeconds(1f);

        BattleEvents.TurnOver();

    }


    public void PassButton()
    {
        // No attacks (or maybe no moves?) so go to next turn...
        if (State == BattleStatesEnum.PLAYER_ATTACK)
        {
            UnhighlightAttackCell();
            BattleEvents.TurnOver();
            GameGrid.Instance.ClearGrid();
        }

    }

    private IEnumerator EnemyMoveCoroutine()
    {

        UIHandler.UpdateStateText("ENEMY MOVE");

        yield return new WaitForSeconds(2f);

        // calculate move...
        

        Enemy thisEnemy = (Enemy)_activeCharacter;

        thisEnemy.CalcMove();

        do
        {
            yield return null;
        } while (_activeCharacter.State == CharacterStatesEnum.MOVING);

        //cell.SetUnit(_activeCharacter);
        //_activeCharacter.UpdatePosition(cell.X, cell.Y, cellPos, _activeCharacter.CurrentFacing);

        BattleEvents.CreatureMoved(_activeCharacter);

        yield return new WaitForSeconds(2f);



        BattleEvents.CreatureMoved(_activeCharacter);

        State = BattleStatesEnum.ENEMY_ATTACK;
        StartCoroutine(EnemyAttackCoroutine());

    }


    private IEnumerator EnemyAttackCoroutine()
    {
        // do attack
        UIHandler.UpdateStateText("ENEMY ATTACK");

        yield return new WaitForSeconds(2f);
        

        
        BattleEvents.TurnOver();

        //CheckBattleState();



    }


    public void Update()
    {

        if (_activeCharacter != null && _activeCharacter.State == CharacterStatesEnum.MOVING)
        {
            CameraHandler.LookAtCreature(_activeCharacter.Transform);
        }

        if (State == BattleStatesEnum.PLAYER_MOVE || State == BattleStatesEnum.PLAYER_ATTACK)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                //if (!EventSystem.current.IsPointerOverGameObject())
                //{
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                int layerMask = 1 << 6;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
                {
                    GameObject hitObject = hitInfo.transform.gameObject;
                    if (hitObject.TryGetComponent(out GridCell selectedCell))
                    {
                        if (selectedCell.IsSelectable)
                        {
                            Debug.Log(selectedCell.name, this);

                            if (State == BattleStatesEnum.PLAYER_MOVE)
                            {
                                BattleEvents.CellMoveSelected(selectedCell);
                            }
                            else
                            {
                                BattleEvents.CellAttackSelected(selectedCell);
                            }
                        }
                    }
                }
            }
        }



        //if (State == BattleStatesEnum.PLAYER_ATTACK)
        //{
        //    if (_currentAttackTemplate != null)
        //    {
        //        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        //        int layerMask = 1 << 6;

        //        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
        //        {
        //            GameObject hitObject = hitInfo.transform.gameObject;
        //            if (hitObject.TryGetComponent(out GridCell selectedCell))
        //            {
        //                // only do the following if this selected cell is different to the selected cell on the previous frame...

        //                Debug.Log("ATTACKING CELL " + selectedCell.name, this);

        //                //_currentAttackTemplate.transform.position = selectedCell.transform.position + new Vector3(0, 0.1f, 0);
        //                //Creature adv = InitiativeManager.GetCurrentCreature();

        //                //List<Creature> attackedCreatures = Grid.GetAttackedCreatures(selectedCell, adv.GetSelectedAttack());
        //                //Debug.Log(attackedCreatures);

        //            }
        //        }
        //    }
        //}


        //Creature c = InitiativeManager.GetCurrentCreature();



    }

    //private void Victory()
    //{
    //    UIHandler.UpdateStateText("VICTORY!");
    //    _gameManager.UpdateState(GameManager.GameStatesEnum.Battle_Victory);

    //}

    private void GoToVictory()
    {
        Debug.Log("EVENT: VICTORY!");
        SceneManager.LoadSceneAsync(3);
    }

    //private void Defeat()
    //{

    //    UIHandler.UpdateStateText("DEFEAT!");
    //    _gameManager.UpdateState(GameManager.GameStatesEnum.Battle_Defeat);

    //    // go to post-run score screen...
    //}

    #region Creature Panel UI

    public void UpdateCreatureText(string text)
    {
        UIHandler.UpdateCharacterText(text);
    }

    public void UpdateStateText(string text)
    {
        UIHandler.UpdateStateText(text);
    }

    #endregion


}

public enum BattleStatesEnum
{
    START,
    START_TURN,
    PLAYER_MOVE,
    PLAYER_ATTACK,
    ENEMY_MOVE,
    ENEMY_ATTACK,
    WON,
    LOST
}
