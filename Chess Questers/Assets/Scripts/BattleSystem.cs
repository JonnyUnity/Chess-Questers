using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class BattleSystem : Singleton<BattleSystem>
{

    public BattleStatesEnum State;

    private GameManager _gameManager;
    private InitiativeManager InitiativeManager;
    public GameGrid Grid;

    [SerializeField]
    private GameObject HeroPrefab;

    private GameObject HeroObj;
    private List<Adventurer> Adventurers;

    [SerializeField]
    private GameObject CreaturePanel;

    private BattleUIHandler UIHandler;

    [SerializeField]
    private GameObject CameraRig;
    private CameraHandler CameraHandler;

    private Camera _camera;

    //[SerializeField]
    //private CinemachineVirtualCamera VirtualCamera;

    //[SerializeField]
    //private CinemachineFreeLook FreeLookCamera;


    // Start is called before the first frame update
    public void Awake()
    {
        _camera = Camera.main;

        State = BattleStatesEnum.START;
        UIHandler = CreaturePanel.GetComponent<BattleUIHandler>();

        CameraHandler = CameraRig.GetComponent<CameraHandler>();

        InitiativeManager = GetComponent<InitiativeManager>();

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

            tmp.Init(advName, 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetRandomSprite(), false, cell);

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
            
            c.Init("Enemy " + (i + 1), 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetRandomSprite(), true, cell);

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
        adv.ToggleSelected(true);

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

        //var attacks = Grid.GetAttacksForPlayer(adv.X, adv.Y);
        var attacks = Grid.GetBaseAttack(adv.OccupiedCell);
        if (attacks.Count == 0)
        {
            // only show pass option...
        }

        // SetupAttackUI...
        

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
        adv.ToggleSelected(false);

        GridCell cell = Grid.GetCell(x, y);
        int damage = adv.GetAttackDamage(null);
        List<Creature> attackedCreatures = Grid.GetAttackedCreatures(cell, null, Vector2.up);

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
