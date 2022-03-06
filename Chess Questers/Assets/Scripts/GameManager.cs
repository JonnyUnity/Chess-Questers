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
    private AttackClass[] AllAttackClasses;
    private Sprite[] Sprites;

    [SerializeField] private GameObject InitiativeTracker;
    private InitiativeManager InitiativeManager;

    [SerializeField] private UIManager _uiManager;

    public GameStatesEnum State { get; private set; }

    // Start is called before the first frame update
    public void Awake()
    {
        AllMoveClasses = Resources.LoadAll<MoveClass>("MoveClasses/");
        AllAttackClasses = Resources.LoadAll<AttackClass>("AttackClasses/");
        Sprites = Resources.LoadAll<Sprite>("Sprites/");

        _uiManager = GetComponent<UIManager>();
    }

    private void Start()
    {
        State = GameStatesEnum.Battle_Start;
        //StartCoroutine(BattleSystem.Instance.SetupCoroutine(this));
    }




    public MoveClass GetRandomMoveClass()
    {
        if (AllMoveClasses.Length == 0)
            return null;

        var playerMoveClasses = AllMoveClasses.Where(w => w.ForPlayer).ToArray();

        int index = Random.Range(0, playerMoveClasses.Length);
        return playerMoveClasses[index];
    }

    public AttackClass[] GetAttacks()
    {
        return AllAttackClasses.Where(w => w.name == "Fireball").ToArray();
    }


    public Sprite GetRandomSprite()
    {
        int index = Random.Range(0, Sprites.Length);
        return Sprites[index];
    }


    public void UpdateState(GameStatesEnum state)
    {
        State = state;

        if (State == GameStatesEnum.Battle_Victory)
        {
            _uiManager.ShowVictoryScreen();
        }
        else if (State == GameStatesEnum.Battle_Defeat)
        {
            _uiManager.ShowDefeatScreen();
        }

    }

    //private void SpawnObstacles()
    //{
    //    for (int i = 0; i < numObstacles; i++)
    //    {
    //        GridCell cell;
    //        int randX, randY;
    //        do
    //        {
    //            //todo : set spawn area where obstacles cannot be spawned...
    //            randX = Random.Range(0, Grid.Width);
    //            randY = Random.Range(0, Grid.Height);
    //            cell = Grid.GetCell(randX, randY);
    //        } while (cell.IsOccupied);
            
    //        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);
    //        _ = Instantiate(ObstaclePrefab, cellPos, Quaternion.identity);
    //        cell.IsOccupied = true;

    //        Debug.Log($"Obstacle added at x:{randX}, y:{randY}");

    //    }
    //}

    //private void SpawnPlayer()
    //{
    //    // pick random coords
    //    GridCell cell = Grid.GetRandomUnoccupiedCell();
    //    Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

    //    AdventurerObj = Instantiate(AdventurerPrefab, cellPos, Quaternion.identity);
    //    Adventurer adv = AdventurerObj.GetComponent<Adventurer>();

    //    //adv.SetMoveClass(GetRandomMoveClass());
    //    //adv.SetMoveClass(AllMoveClasses.Where(w => w.name == "Knight").Single());
    //    adv.SetPosition(cell.X, cell.Y);

    //   // UpdateAdventurerMoves(adv);

    //}

    //private void SpawnPlayers()
    //{
    //    foreach (Adventurer adv in PartyManager.Instance.Heroes)
    //    {
    //        // pick random coords
    //        GridCell cell = Grid.GetRandomUnoccupiedCell();
    //        Vector3 cellPos = Grid.GetGridCellWorldPosition(cell);

    //        AdventurerObj = Instantiate(AdventurerPrefab, cellPos, Quaternion.identity);
    //        AdventurerObj.name = adv.name;
    //        Adventurer tmp = AdventurerObj.GetComponent<Adventurer>();

    //        // copy values
    //        tmp.name = adv.name;
    //        //tmp.moveClass = adv.moveClass;

    //        tmp.SetPosition(cell.X, cell.Y);

    //        //UpdateAdventurerMoves(tmp);

    //    }

    //    PartyManager.Instance.CurrHero = PartyManager.Instance.Heroes.First();
    //    UpdateAdventurerMoves();

    //}


    //private void UpdateAdventurerMoves()
    //{
    //    //adv.SetPosition(0, 0);
    //    //adv.name = "Hello there!";
    //    var adv = PartyManager.Instance.CurrHero;
    //    UIManager.Instance.SetAdventurerText(adv.name);
    //    Grid.SetPossibleMovesForPlayer(adv);
    //}

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