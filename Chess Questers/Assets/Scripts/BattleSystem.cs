using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleSystem : Singleton<BattleSystem>
{

    private int _id;
    private int _layout;

    public BattleStatesEnum State;

    private GameManager _gameManager;
    private InitiativeManager InitiativeManager;
    public GameGrid Grid;

    [SerializeField] private GameObject HeroPrefab;

    private GameObject HeroObj;
    private List<Adventurer> Adventurers;

    [SerializeField] private GameObject CreaturePanel;

    private BattleUIHandler UIHandler;

    [SerializeField] private GameObject CameraRig;
    private CameraHandler CameraHandler;

    private Camera _camera;

    [SerializeField] private GameObject _attackPrefab;
    private GameObject _currentAttackTemplate;

    //[SerializeField]
    //private CinemachineVirtualCamera VirtualCamera;

    //[SerializeField]
    //private CinemachineFreeLook FreeLookCamera;

    private QuestData _questData;
    private List<ImprovedCharacter> NewAdventurers;
    private List<ImprovedCharacter> NewEnemies;
    private List<ImprovedCharacter> Combatants;

    [SerializeField] private NewIM IM;


    // Start is called before the first frame update
    public void Awake()
    {
        _camera = Camera.main;

        State = BattleStatesEnum.START;
        UIHandler = CreaturePanel.GetComponent<BattleUIHandler>();

        CameraHandler = CameraRig.GetComponent<CameraHandler>();

        InitiativeManager = GetComponent<InitiativeManager>();

        //EventSystem.OnBattleStarted += BattleStarted;
        EventSystem.OnBattleVictory += GoToVictory;
        EventSystem.OnTurnStart += StartNextTurn;


    }

    private void Start()
    {
        // load data here that needs to happen on scene load regardless of whether it's the player
        // starting the encounter, or resuming from save
        
        _questData = NewGameManager.Instance.GetQuestData();

        _id = _questData.Battle_ID;
        _layout = _questData.Battle_Layout;

        //var battleData = _questData.BattleData;

        // create Character objects from data.
        //NewAdventurers = SaveDataManager.DeserializeCharacterData(_questData.PartyMembers, true);
        //NewEnemies = SaveDataManager.DeserializeCharacterData(battleData.Enemies, false);

        NewAdventurers = _questData.PartyMembers;
        NewEnemies = _questData.Enemies;
        Combatants = new List<ImprovedCharacter>();
        Combatants.AddRange(_questData.PartyMembers);
        Combatants.AddRange(_questData.Enemies);

        // create a specific grid dependent on battle encounter data.
        StartCoroutine(Grid.CreateGameGridCoroutine(this));

        IM.SetInitiative(_questData.Initiative);
        
        if (IM.HasCombatStarted())
        {
            // resume battle...
            StartCoroutine(ResumeBattleCoroutine());

        }
        else
        {

            
            StartCoroutine(InitBattleCoroutine());

        }
        

    }

    private void BattleStarted()
    {
        Debug.Log("Battle started!");
        State = BattleStatesEnum.START;


        StartCoroutine(nameof(InitBattleCoroutine));


    }


    private void StartNextTurn(int characterID)
    {
        Debug.Log("BattleSystem - Start Next Turn! " + characterID);
        State = BattleStatesEnum.START_TURN;

        ImprovedCharacter c = Combatants.Where(w => w.ID == characterID).Single();

       // CameraHandler.SwapToCharacter(c.Position);


        UIHandler.UpdateCharacterText(c.Name);

    }


    private IEnumerator InitBattleCoroutine()
    {

        Debug.Log("Play opening animation");
        //yield return new WaitForSeconds(1f);


        Encounter encounter = NewGameManager.Instance.GetEncounter(_questData.Battle_ID);

        EventSystem.RollInitiative(Combatants);

        for (int i = 0; i < NewAdventurers.Count; i++)
        {
            Vector2 spawn = encounter.PlayerSpawns[i];
            GridCell cell = Grid.GetCell((int)spawn.x, (int)spawn.y);
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            Quaternion rot;

            ImprovedCharacter adv = NewAdventurers[i];

            adv.CurrentFacing = 0;
                rot = Quaternion.Euler(0, -90, 0);

            adv.UpdatePosition(cell.X, cell.Y, cellPos, 0);

            HeroObj = Instantiate(HeroPrefab, cellPos, Quaternion.identity * rot);
            HeroObj.name = adv.Name;

            //cell.SetUnit(adv);

        }

        for (int i = 0; i < NewEnemies.Count; i++)
        {
            Vector2 spawn = encounter.EnemySpawns[i];
            GridCell cell = Grid.GetCell((int)spawn.x, (int)spawn.y);
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            Quaternion rot;

            ImprovedCharacter adv = NewEnemies[i];

            adv.CurrentFacing = 2;
            rot = Quaternion.Euler(0, 90, 0);

            adv.UpdatePosition(cell.X, cell.Y, cellPos, 0);

            HeroObj = Instantiate(HeroPrefab, cellPos, Quaternion.identity * rot);
            HeroObj.name = adv.Name;

            //cell.SetUnit(adv);

        }

        yield return null;
    }

    private IEnumerator ResumeBattleCoroutine()
    {

        // set character positions

        yield return null;
    }

    public void SaveQuest()
    {
        StartCoroutine(SaveProgress());

    }

    public IEnumerator SaveProgress()
    {

        _questData.Battle_ID = _id;
        _questData.Battle_Layout = _layout;
        _questData.Initiative = IM._init;
        _questData.PartyMembers = NewAdventurers;
        _questData.Enemies = NewEnemies;

        NewGameManager.Instance.SaveQuest(_questData);

        yield return null;
    }


    public IEnumerator SetupCoroutine(GameManager gameManager)
    {
        _gameManager = gameManager;

        // setup grid
        yield return StartCoroutine(Grid.CreateGameGridCoroutine(this));

       
        UIHandler.UpdateStateText("BATTLE START");

        // Spawn adventurers!
        foreach (string advName in PartyManager.Instance.DefaultNames)
        {
            // pick random coords
            GridCell cell = Grid.GetRandomUnoccupiedCell();
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            HeroObj = Instantiate(HeroPrefab, cellPos, Quaternion.identity * Quaternion.Euler(0, -90, 0));
            HeroObj.name = advName;
            
            Creature tmp = HeroObj.GetComponent<Creature>();
            tmp.SetColour(Color.green);

            tmp.Init(advName, 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetAttacks(), GameManager.Instance.GetRandomSprite(), false, cell);

            cell.SetUnit(tmp);

            PartyManager.Instance.Heroes.Add(tmp);

        }

        // Spawn enemies
        List<Creature> creatures = new();
        Creature c;
        for (int i = 0; i < 3; i++)
        {
            GridCell cell = Grid.GetRandomUnoccupiedCell();
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            GameObject obj = Instantiate(HeroPrefab, cellPos, Quaternion.identity);

            c = obj.GetComponent<Creature>();
            c.SetColour(Color.red);
            
            c.Init("Enemy " + (i + 1), 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetAttacks(), GameManager.Instance.GetRandomSprite(), true, cell);

            cell.SetUnit(c);

            creatures.Add(c);
        }

        InitiativeManager.Setup(PartyManager.Instance.Heroes, creatures);
       
        c = InitiativeManager.StartInitiative();

        CameraHandler.SwapToCharacter(c.transform);


        UIHandler.UpdateCharacterText(c.CreatureName);

        if (c.IsEnemy)
        {
            State = BattleStatesEnum.ENEMY_MOVE;
            StartCoroutine(EnemyMoveCoroutine());
        }
        else
        {
            State = BattleStatesEnum.PLAYER_MOVE;
            SetupPlayerMove();
        }
    }


    //public void OnGridSelection(int x, int y)
    //{
    //    if (State == BattleStatesEnum.PLAYER_MOVE)
    //    {
    //        // do move...
    //        StartCoroutine(PlayerMoveCoroutine(x, y));

    //    }
    //    else if (State == BattleStatesEnum.PLAYER_ATTACK)
    //    {
    //        // do attack
    //        StartCoroutine(PlayerAttackCoroutine(x, y));

    //    }
    //}

    public void OnGridCellClick(GridCell cell)
    {
        if (State == BattleStatesEnum.PLAYER_MOVE && cell.IsMove)
        {
            StartCoroutine(PlayerMoveCoroutine(cell.X, cell.Y));
        }
        else if (State == BattleStatesEnum.PLAYER_ATTACK && cell.IsAttack)
        {
            StartCoroutine(PlayerAttackCoroutine(cell.X, cell.Y));
        }
    }

    void SetupPlayerMove()
    {
        UIHandler.UpdateStateText("CALC PLAYER MOVES");
        Creature adv = InitiativeManager.GetCurrentCreature();

        //Grid.GetMovesForPlayer(adv.MoveClass, adv.X, adv.Y);
        Grid.GetMovesForPlayerNew(adv.MoveClass, adv.OccupiedCell);


        UIHandler.UpdateCharacterText(adv.CreatureName);
        UIHandler.UpdateStateText("WAITING FOR PLAYER MOVE");
        
    }

    public void CharacterLook(int x, int y)
    {
        Creature adv = InitiativeManager.GetCurrentCreature();
        GridCell cell = Grid.GetCell(x, y);
        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);
        adv.LookAtCell(cellPos);


    }

    IEnumerator PlayerMoveCoroutine(int x, int y)
    {
        Grid.ClearGrid();

        UIHandler.UpdateStateText("PLAYER MOVE");

        GridCell cell = Grid.GetCell(x, y);
        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

        Creature adv = InitiativeManager.GetCurrentCreature();
        adv.OccupiedCell.ResetCell();

        adv.DoMove(cellPos, x, y);
        
        do
        {
            yield return null;
        } while (adv.State == CreatureStatesEnum.MOVING);

        cell.SetUnit(adv);

        yield return new WaitForSeconds(0.1f);

        State = BattleStatesEnum.PLAYER_ATTACK;

        UIHandler.BuildAttackList(adv);

    }


    void SetupPlayerAttack()
    {
        UIHandler.UpdateStateText("CALC PLAYER ATTACKS");
        Creature adv = InitiativeManager.GetCurrentCreature();
        adv.SetSelectedAttack(0);


        //var attacks = Grid.GetAttacksForPlayer(adv.X, adv.Y);
        var attacks = Grid.GetBaseAttack(adv.OccupiedCell);
        if (attacks.Count == 0)
        {
            // only show pass option...
        }

        // SetupAttackUI...

        _currentAttackTemplate = Instantiate(_attackPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        

        

        UIHandler.UpdateCharacterText(adv.CreatureName);
        UIHandler.UpdateStateText("WAITING FOR PLAYER ATTACK");
    }


    void TestSetupFireball()
    {
        UIHandler.UpdateStateText("CALC PLAYER ATTACKS");
        Creature adv = InitiativeManager.GetCurrentCreature();

        //var attacks = Grid.GetAttacksForPlayer(adv.X, adv.Y);
        var attacks = Grid.GetFireballAttack(adv.OccupiedCell);
        if (attacks.Count == 0)
        {
            // only show pass option...
        }

        UIHandler.UpdateCharacterText(adv.CreatureName);
        UIHandler.UpdateStateText("WAITING FOR PLAYER ATTACK");
    }

    IEnumerator PlayerAttackCoroutine(int x, int y)
    {
        Grid.ClearGrid();

        // do attack!
        UIHandler.UpdateStateText("PLAYER ATTACK");

        Creature adv = InitiativeManager.GetCurrentCreature();


        GridCell cell = Grid.GetCell(x, y);
        int damage = adv.GetAttackDamage(null);
        List<Creature> attackedCreatures = Grid.GetAttackedCreatures(cell, null);

        foreach (Creature c in attackedCreatures)
        {
            if (c.TryGetComponent(out HealthSystem health))
            {
                health.ChangeHealth(-damage);
            }
        }

        yield return new WaitForSeconds(2f);

        CheckBattleState();
    }

    void CheckBattleState()
    {
        // change state based on what happened...
        if (InitiativeManager.AreAllEnemiesDead())
        {
            Victory();            
            return;
        }
        if (InitiativeManager.AreAllHeroesDead())
        {
            
            Defeat();
            return;
        }

        // if all enemies dead, then victory, else next combatant
        InitiativeManager.NextTurn();

        Creature c = InitiativeManager.GetCurrentCreature();

        CameraHandler.SwapToCharacter(c.transform);
        
        if (InitiativeManager.IsEnemyTurn())
        {
            State = BattleStatesEnum.ENEMY_MOVE;
            StartCoroutine(EnemyMoveCoroutine());
        }
        else
        {
            State = BattleStatesEnum.PLAYER_MOVE;
            SetupPlayerMove();
        }

    }

    public void OnAttack1()
    {
        SetupPlayerAttack();
    }

    public void OnAttack2()
    {
        TestSetupFireball();
    }

    public void PassButton()
    {
        // No attacks (or maybe no moves?) so go to next turn...
        if (State == BattleStatesEnum.PLAYER_ATTACK)
        {
            CheckBattleState();
        }
    }

    private IEnumerator EnemyMoveCoroutine()
    {
        // calculate move...
        UIHandler.UpdateStateText("ENEMY MOVE");

        yield return new WaitForSeconds(2f);
        State = BattleStatesEnum.ENEMY_ATTACK;
        StartCoroutine(EnemyAttackCoroutine());

    }


    private IEnumerator EnemyAttackCoroutine()
    {
        // do attack
        UIHandler.UpdateStateText("ENEMY ATTACK");

        yield return new WaitForSeconds(2f);

        CheckBattleState();
    }


    public void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                int layerMask = 1 << 6;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
                {
                    GameObject hitObject = hitInfo.transform.gameObject;
                    if (hitObject.TryGetComponent(out GridCell selectedCell))
                    {
                        Debug.Log(selectedCell.name, this);
                        OnGridCellClick(selectedCell);
                    }
                }
            //}

            
        }

        if (State == BattleStatesEnum.PLAYER_ATTACK)
        {
            if (_currentAttackTemplate != null)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                int layerMask = 1 << 6;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
                {
                    GameObject hitObject = hitInfo.transform.gameObject;
                    if (hitObject.TryGetComponent(out GridCell selectedCell))
                    {
                        // only do the following if this selected cell is different to the selected cell on the previous frame...

                        Debug.Log("ATTACKING CELL " + selectedCell.name, this);

                        _currentAttackTemplate.transform.position = selectedCell.transform.position + new Vector3(0, 0.1f, 0);
                        Creature adv = InitiativeManager.GetCurrentCreature();

                        List<Creature> attackedCreatures = Grid.GetAttackedCreatures(selectedCell, adv.GetSelectedAttack());
                        Debug.Log(attackedCreatures);

                    }
                }
            }
        }


        Creature c = InitiativeManager.GetCurrentCreature();

        if (c != null && c.State == CreatureStatesEnum.MOVING)
        {
            CameraHandler.LookAtCreature(InitiativeManager.GetCurrentCreature().transform);
        }

    }

    private void Victory()
    {
        UIHandler.UpdateStateText("VICTORY!");
        _gameManager.UpdateState(GameManager.GameStatesEnum.Battle_Victory);

    }

    private void GoToVictory()
    {
        Debug.Log("EVENT: VICTORY!");
        SceneManager.LoadScene(4);
    }

    private void Defeat()
    {

        UIHandler.UpdateStateText("DEFEAT!");
        _gameManager.UpdateState(GameManager.GameStatesEnum.Battle_Defeat);

        // go to post-run score screen...
    }

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
