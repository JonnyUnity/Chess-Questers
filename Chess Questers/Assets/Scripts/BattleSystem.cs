using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleSystem : MonoBehaviour
{

    public BattleStatesEnum State;

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

    //[SerializeField]
    //private CinemachineVirtualCamera VirtualCamera;

    //[SerializeField]
    //private CinemachineFreeLook FreeLookCamera;


    // Start is called before the first frame update
    public void Awake()
    {
        State = BattleStatesEnum.START;
        UIHandler = CreaturePanel.GetComponent<BattleUIHandler>();

        CameraHandler = CameraRig.GetComponent<CameraHandler>();

    }


    public IEnumerator Setup(InitiativeManager initiativeManager)
    {

        // setup grid
        yield return StartCoroutine(Grid.CreateGameGrid(this));

        InitiativeManager = initiativeManager;

        
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
            creatures.Add(c);
        }

        InitiativeManager.Setup(PartyManager.Instance.Heroes, creatures);
       
        c = InitiativeManager.StartInitiative();

        CameraHandler.LookAtCreature(c.transform);

        //VirtualCamera.Follow = c.transform;
        //VirtualCamera.LookAt = c.transform;
        //FreeLookCamera.Follow = c.transform;
        //FreeLookCamera.LookAt = c.transform;

        UIHandler.UpdateCharacterText(c.CreatureName);

        if (c.IsEnemy)
        {
            State = BattleStatesEnum.ENEMY_MOVE;
            StartCoroutine(EnemyMove());
        }
        else
        {
            State = BattleStatesEnum.PLAYER_MOVE;
            SetupPlayerMove();
        }
    }


    public void OnGridSelection(int x, int y)
    {
        if (State == BattleStatesEnum.PLAYER_MOVE)
        {
            // do move...
            StartCoroutine(PlayerMove(x, y));

        }
        else if (State == BattleStatesEnum.PLAYER_ATTACK)
        {
            // do attack
            StartCoroutine(PlayerAttack());

        }
    }

    void SetupPlayerMove()
    {
        UIHandler.UpdateStateText("CALC PLAYER MOVES");
        Creature adv = InitiativeManager.GetCurrentCreature();

        Grid.GetMovesForPlayer(adv.MoveClass, adv.X, adv.Y);
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

    IEnumerator PlayerMove(int x, int y)
    {
        Grid.ClearGrid();

        UIHandler.UpdateStateText("PLAYER MOVE");

        GridCell cell = Grid.GetCell(x, y);
        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

        Creature adv = InitiativeManager.GetCurrentCreature();
        int startX = adv.X;
        int startY = adv.Y;
        adv.DoMove(cellPos, x, y);

        do
        {
            yield return null;
        } while (adv.State == CreatureStatesEnum.MOVING);

        Grid.UpdateGridOfMove(startX, startY, x, y);

        yield return new WaitForSeconds(0.1f);

        State = BattleStatesEnum.PLAYER_ATTACK;

        UIHandler.BuildAttackList(adv);

    }


    void SetupPlayerAttack()
    {
        UIHandler.UpdateStateText("CALC PLAYER ATTACKS");
        Creature adv = InitiativeManager.GetCurrentCreature();

        var attacks = Grid.GetAttacksForPlayer(adv.X, adv.Y);
        if (attacks.Count == 0)
        {
            // only show pass option...
        }

        // SetupAttackUI...
        

        UIHandler.UpdateCharacterText(adv.CreatureName);
        UIHandler.UpdateStateText("WAITING FOR PLAYER ATTACK");
    }

    IEnumerator PlayerAttack()
    {
        Grid.ClearGrid();

        // do attack!
        UIHandler.UpdateStateText("PLAYER ATTACK");

        Creature adv = InitiativeManager.GetCurrentCreature();
        adv.ToggleSelected(false);

        yield return new WaitForSeconds(2f);


        


        CheckBattleState();
    }

    void CheckBattleState()
    {
        // change state based on what happened...
        // if all enemies dead, then victory, else next combatant
        InitiativeManager.NextTurn();

        Creature c = InitiativeManager.GetCurrentCreature();

        CameraHandler.LookAtCreature(c.transform);
        
        if (InitiativeManager.IsEnemyTurn())
        {
            State = BattleStatesEnum.ENEMY_MOVE;
            StartCoroutine(EnemyMove());
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

    public void PassButton()
    {
        // No attacks (or maybe no moves?) so go to next turn...
        if (State == BattleStatesEnum.PLAYER_ATTACK)
        {
            CheckBattleState();
        }
    }

    private IEnumerator EnemyMove()
    {
        // calculate move...
        UIHandler.UpdateStateText("ENEMY MOVE");

        yield return new WaitForSeconds(2f);
        State = BattleStatesEnum.ENEMY_ATTACK;
        StartCoroutine(EnemyAttack());

    }


    private IEnumerator EnemyAttack()
    {
        // do attack
        UIHandler.UpdateStateText("ENEMY ATTACK");

        yield return new WaitForSeconds(2f);

        CheckBattleState();
    }


    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << 6;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, layerMask))
        {
            GameObject hitObject = hitInfo.transform.gameObject;

        }

        Creature c = InitiativeManager.GetCurrentCreature();

        if (c != null && c.State == CreatureStatesEnum.MOVING)
        {
            CameraHandler.LookAtCreature(InitiativeManager.GetCurrentCreature().transform);
        }

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
