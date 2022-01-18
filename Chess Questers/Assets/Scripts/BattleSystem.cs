using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    public BattleStatesEnum State;

    private InitiativeManager InitiativeManager;
    public GameGrid Grid;

    [SerializeField]
    private GameObject AdventurerPrefab;

    private GameObject AdventurerObj;
    private List<Adventurer> Adventurers;

    [SerializeField]
    private GameObject CreaturePanel;

    private BattleUIHandler UIHandler;



    // Start is called before the first frame update
    public void Awake()
    {
        State = BattleStatesEnum.START;
        UIHandler = CreaturePanel.GetComponent<BattleUIHandler>();
    }


    public IEnumerator Setup(InitiativeManager initiativeManager)
    {

        yield return StartCoroutine(Grid.CreateGameGrid(this));

        InitiativeManager = initiativeManager;

        // setup grid
        UIHandler.UpdateStateText("BATTLE START");

        // Spawn adventurers!
        foreach (string advName in PartyManager.Instance.DefaultNames)
        {
            // pick random coords
            GridCell cell = Grid.GetRandomUnoccupiedCell();
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            AdventurerObj = Instantiate(AdventurerPrefab, cellPos, Quaternion.identity);
            AdventurerObj.name = advName;
            
            Creature tmp = AdventurerObj.GetComponent<Creature>();
            tmp.SetColour(Color.green);

            tmp.Init(advName, 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetRandomSprite(), false, cell);

            PartyManager.Instance.Adventurers.Add(tmp);

        }

        // Spawn enemies
        List<Creature> creatures = new();
        Creature c;
        for (int i = 0; i < 3; i++)
        {
            GridCell cell = Grid.GetRandomUnoccupiedCell();
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            GameObject obj = Instantiate(AdventurerPrefab, cellPos, Quaternion.identity);
            
            c = obj.GetComponent<Creature>();
            c.SetColour(Color.red);
            
            c.Init("Enemy " + (i + 1), 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetRandomSprite(), true, cell);
            creatures.Add(c);
        }

        InitiativeManager.Setup(PartyManager.Instance.Adventurers, creatures);
       
        c = InitiativeManager.StartInitiative();
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

        Grid.SetPossibleMovesForPlayer(adv);
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
        //SetupPlayerAttack();

        // SetupAttackUI...
        UIHandler.BuildAttackList(adv);

    }


    void SetupPlayerAttack()
    {
        UIHandler.UpdateStateText("CALC PLAYER ATTACKS");
        Creature adv = InitiativeManager.GetCurrentCreature();

        //Grid.SetPossibleAttacksForPlayer(adv);

        var attacks = Grid.GetAttacksForPlayer(adv);
        if (attacks.Count == 0)
        {

        }

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


        // DEBUG
        InitiativeManager.NextTurn();
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

        //if (InitiativeManager.AllEnemiesDead())
        //{
        //    State = BattleStatesEnum.WON;
        //    EndBattle();
        //}
        //else if (InitiativeManager.AllAdventurersDead())
        //{
        //    State = BattleStatesEnum.LOST;
        //    EndBattle();
        //}
        //else
        //{
        //    // battle continues...
        //    InitiativeManager.NextInitiative();
        //    Creature c = InitiativeManager.GetCurrentCreature();
        //    if (c.IsEnemy)
        //    {
        //        State = BattleStatesEnum.ENEMY_MOVE;
        //        //EnemyMove();
        //    }
        //    else
        //    {
        //        State = BattleStatesEnum.PLAYER_MOVE;
        //        //PlayerMove();
        //    }

        //}
    }

    public void OnAttack1()
    {
        //StartCoroutine(PlayerAttack());
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

    void EndBattle()
    {
        UIHandler.UpdateStateText("END BATTLE");


        if (State == BattleStatesEnum.WON)
        {

        }
        else
        {

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
