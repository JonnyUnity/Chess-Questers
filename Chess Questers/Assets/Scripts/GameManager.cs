using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{

    private GameGrid Grid;
    [SerializeField]
    private GameObject AdventurerPrefab;

    [SerializeField]
    private GameObject ObstaclePrefab;
    [SerializeField]
    private int numObstacles;

    private GameObject AdventurerObj;

    private MoveClass[] AllMoveClasses;
    private Sprite[] Sprites;

    [SerializeField] private GameObject InitiativeTracker;
    private InitiativeManager InitiativeManager;
    private BattleSystem BattleSystem;

    private GameState State;

    private Initiative TurnOrder;

    // Start is called before the first frame update
    public void Awake()
    {
        AllMoveClasses = Resources.LoadAll<MoveClass>("MoveClasses/");

        Sprites = Resources.LoadAll<Sprite>("Sprites/");

        //Grid = FindObjectOfType<GameGrid>();
        //Grid.CreateGameGrid();

        //PartyManager.Instance.

        //BattleSystem = gameObject.AddComponent<BattleSystem>();
        BattleSystem = GetComponentInChildren<BattleSystem>();
        InitiativeManager = BattleSystem.gameObject.AddComponent<InitiativeManager>();

        StartCoroutine(BattleSystem.Setup(InitiativeManager));


        //PartyManager.Instance.CreateAdventurers(bs, 3);

        // Spawn obstacles
        //SpawnObstacles();

        // Spawn player
        //SpawnPlayer();

        //SpawnPlayers();


        //InitiativeTest();

    //    SetupBattle();
    }

    
    public MoveClass GetRandomMoveClass()
    {
        if (AllMoveClasses.Length == 0)
            return null;

        var playerMoveClasses = AllMoveClasses.Where(w => w.ForPlayer).ToArray();

        int index = Random.Range(0, playerMoveClasses.Length);
        return playerMoveClasses[index];
    }

    public Sprite GetRandomSprite()
    {
        int index = Random.Range(0, Sprites.Length);
        return Sprites[index];
    }

    private void SpawnObstacles()
    {
        for (int i = 0; i < numObstacles; i++)
        {
            GridCell cell;
            int randX, randY;
            do
            {
                //todo : set spawn area where obstacles cannot be spawned...
                randX = Random.Range(0, Grid.Width);
                randY = Random.Range(0, Grid.Height);
                cell = Grid.GetCell(randX, randY);
            } while (cell.IsOccupied);
            
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);
            _ = Instantiate(ObstaclePrefab, cellPos, Quaternion.identity);
            cell.IsOccupied = true;

            Debug.Log($"Obstacle added at x:{randX}, y:{randY}");

        }
    }

    private void SpawnPlayer()
    {
        // pick random coords
        GridCell cell = Grid.GetRandomUnoccupiedCell();
        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

        AdventurerObj = Instantiate(AdventurerPrefab, cellPos, Quaternion.identity);
        Adventurer adv = AdventurerObj.GetComponent<Adventurer>();

        //adv.SetMoveClass(GetRandomMoveClass());
        //adv.SetMoveClass(AllMoveClasses.Where(w => w.name == "Knight").Single());
        adv.SetPosition(cell.X, cell.Y);

       // UpdateAdventurerMoves(adv);

    }

    private void SpawnPlayers()
    {
        foreach (Adventurer adv in PartyManager.Instance.Adventurers)
        {
            // pick random coords
            GridCell cell = Grid.GetRandomUnoccupiedCell();
            Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

            AdventurerObj = Instantiate(AdventurerPrefab, cellPos, Quaternion.identity);
            AdventurerObj.name = adv.name;
            Adventurer tmp = AdventurerObj.GetComponent<Adventurer>();

            // copy values
            tmp.name = adv.name;
            //tmp.moveClass = adv.moveClass;

            tmp.SetPosition(cell.X, cell.Y);

            //UpdateAdventurerMoves(tmp);

        }

        PartyManager.Instance.CurrAdventurer = PartyManager.Instance.Adventurers.First();
        UpdateAdventurerMoves();

    }


    private void UpdateAdventurerMoves()
    {
        //adv.SetPosition(0, 0);
        //adv.name = "Hello there!";
        var adv = PartyManager.Instance.CurrAdventurer;
        UIManager.Instance.SetAdventurerText(adv.name);
        Grid.SetPossibleMovesForPlayer(adv);
    }

    private void UpdateAdventurerAttack()
    {
        var adv = PartyManager.Instance.CurrAdventurer;
        Grid.SetPossibleAttacksForPlayer(adv);
    }

    public void MovePlayer(int x, int y)
    {
        GridCell cell = Grid.GetCell(x, y);
        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

        AdventurerObj.transform.SetPositionAndRotation(cellPos, Quaternion.identity);
        Adventurer adv = AdventurerObj.GetComponent<Adventurer>();
        adv.SetPosition(x, y);

       // UpdateAdventurerMoves(adv);

        State = GameState.PlayerAttack;
    }






}

public enum GameState
{
    PlayerTurn,
    PlayerAttack,
    EnemyTurn,
    GameOver,
    BattleVictory,
    RunVictory
}