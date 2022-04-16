using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameGrid : Singleton<GameGrid>
{
    public readonly int Height = 10;
    public readonly int Width = 10;
    private float GridSpacesize = 5f;

    private int NumObstacles;

    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _cellsContainer;

    private GameObject GridCellPrefab;
    private GameObject ObstaclePrefab;
    private GameObject[,] Grid;

    [SerializeField] private Color lightColor;
    [SerializeField] private Color darkColor;

    [SerializeField] private GameObject _attackTemplatePrefab;

    private int[][] numSquaresToEdge;
    private List<(int, int)> slideOffsets;
    private List<(int, int)> knightOffsets;

    private List<(int, int)> baseAttack = new List<(int, int)>() { (1, 0), (-1, 0), (0, 1), (0, -1) };
    private List<(int, int)> _cellNeighbourDirections = new() { (1, 0), (-1, 0), (0, 1), (0, -1) };


    public Dictionary<int, Vector2> PlayerCharacterPositions { get; private set; }
    public Dictionary<int, Vector2> EnemyPositions { get; private set; }


    [SerializeField] private CreatureRuntimeSet _playerCharacters;
    [SerializeField] private CreatureRuntimeSet _enemies;
    [SerializeField] private CreatureRuntimeSet _combatants;
    [SerializeField] private CreatureRuntimeSet _targets;

    [SerializeField] private GridCellRuntimeSet _targetCells;

    [SerializeField] private ActionResult _playerAction;

    private List<ActionResult> _actionResults;

    public void Awake()
    {
        PlayerCharacterPositions = new Dictionary<int, Vector2>();
        EnemyPositions = new Dictionary<int, Vector2>();

        GridCellPrefab = Resources.Load("Prefabs/NewGridCell") as GameObject;
        ObstaclePrefab = Resources.Load("Prefabs/Obstacle") as GameObject;

        BattleEvents.OnPlayerActionSelected += ShowActionOnGrid;
        BattleEvents.OnCreatureMoved += UpdateCreaturePosition;
        BattleEvents.OnPlayerStartTurn += ShowGrid;
        BattleEvents.OnPlayerEndTurn += HideGrid;
        BattleEvents.OnCellSelected += SelectCell;

    }


    private void OnDisable()
    {
        BattleEvents.OnPlayerActionSelected -= ShowActionOnGrid;
        BattleEvents.OnCreatureMoved -= UpdateCreaturePosition;
        BattleEvents.OnPlayerStartTurn -= ShowGrid;
        BattleEvents.OnPlayerEndTurn -= HideGrid;
        BattleEvents.OnCellSelected -= SelectCell;
    }

    private void SelectCell(GridCell cell)
    {
        if (cell.IsMove)
        {
            BattleEvents.CellMoveSelected(cell);
        }
        else if (cell.IsAttack)
        {
            _playerAction.Cell = cell;
            _playerAction.Creatures = GetAttackedCreatures(cell, _playerAction.Action);

            BattleEvents.CellAttackSelected(cell);
            //CreatureRuntimeSet creatures = _playerAction.Action.IsAttack;
        }

        
    }

    private void HighlightCell(GridCell cell)
    {
        if (cell.IsMove)
        {
            BattleEvents.CellMoveHighlighted(cell);
        }
        else if (cell.IsAttack)
        {
            BattleEvents.CellAttackHighlighted(cell);
        }

    }

    private void UnhighlightCell(GridCell cell)
    {
        //Debug.Log("Unhighlighting cell: " + cell.ToString());
        BattleEvents.CellUnhighlighted();
    }


    private void ShowActionOnGrid(ActionClass action, CreatureRuntimeSet creatures, int x, int y)
    {

        ClearGrid();

        // get attack template from action
        Debug.Log("Action: " + action.Name + " selected!");



        if (action.IsRanged)
        {

            int minRange = action.MinRange;
            int maxRange = action.MaxRange;

            for (int i = x - maxRange; i <= x + maxRange; i++)
            {
                for (int j = y - maxRange; j <= y + maxRange; j++)
                {
                    if (0 <= i && i < Width && 0 <= j && j < Height)
                    {

                        
                        int cellChebyshevDistance = CalculateChebyshevDistance(x, i, y, j);
                        //Debug.Log($"({i},{j}) = {cellChebyshevDistance}");

                        if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange)
                        {
                            //Grid[i, j].gameObject.SetActive(true);
                            GridCell cell = Grid[i, j].GetComponent<GridCell>();
                            cell.SetAsValidAttack();
                        }
                    }

                }
            }
        }
        else // melee
        {
            // Check only the cells immediately adjacent to the active character
            // only set a cell as a valid attack if it is occupied.
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    if (0 <= i && i < Width && 0 <= j && j < Height)
                    {
                        
                        if (IsCellOccupied(creatures, i, j))
                        {
                            //Grid[i, j].gameObject.SetActive(true);
                            GridCell cell = Grid[i, j].GetComponent<GridCell>();
                            cell.SetAsValidAttack();
                        }


                        //int cellChebyshevDistance = CalculateChebyshevDistance(x, i, y, j);
                        ////Debug.Log($"({i},{j}) = {cellChebyshevDistance}");

                        //if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange)
                        //{
                        //    cell.SetAsValidAttack();
                        //}
                    }
                }
            }

        }



    }

    public void CreateGameGrid()
    {
        Grid = new GameObject[Height, Width];

        //yield return new WaitForSeconds(1f);

        if (GridCellPrefab == null)
        {
            Debug.Log("ERROR: Grid cell prefab not set!");
        }

        for (int x = 0; x < Height; x++)
        {
            for (int y = 0; y < Width; y++)
            {
                Vector3 cellPosition = new Vector3(x * GridSpacesize, 0.01f, y * GridSpacesize);

                Grid[x, y] = Instantiate(GridCellPrefab, cellPosition, Quaternion.Euler(90, 0, 0), _cellsContainer.transform);

                bool isLightSquare = (x + y) % 2 != 0;
                var squareColour = isLightSquare ? lightColor : darkColor;
                GridCell cell = GetCell(x, y);
                cell.Setup(x, y, cellPosition, squareColour);
                //cell.transform.parent = _cellsContainer.transform;
                cell.name = $"Grid Space (x:{x}, y:{y})";
                //Grid[x, y].gameObject.SetActive(false);
            }
        }

        ComputeMoveData();
        SpawnObstacles();


    }

    public void ShowGrid()
    {
        _cellsContainer.SetActive(true);
    }

    public void HideGrid()
    {
        _cellsContainer.SetActive(false);
    }


    private void ComputeMoveData()
    {

        // First 4 are orthogonal, last 4 are diagonals (N, S, W, E, NW, SE, NE, SW)
        slideOffsets = new List<(int, int)>() { (0, 1), (0, -1), (-1, 0), (1, 0), (-1, 1), (1, -1), (1, 1), (-1, -1) };
        knightOffsets = new List<(int, int)>() { (1, 2), (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2) };

        int totalSquares = Width * Height;
        numSquaresToEdge = new int[totalSquares][];

        // First 4 are orthogonal, last 4 are diagonals (N, S, W, E, NW, SE, NE, SW)
        for (int squareIndex = 0; squareIndex < totalSquares; squareIndex++)
        {
            int x = squareIndex / Width;
            int y = squareIndex - x * Width;

            int north = Height - x - 1;
            int south = x;
            int west = y;
            int east = Width - y - 1;
            numSquaresToEdge[squareIndex] = new int[8];
            numSquaresToEdge[squareIndex][0] = north;
            numSquaresToEdge[squareIndex][1] = south;
            numSquaresToEdge[squareIndex][2] = west;
            numSquaresToEdge[squareIndex][3] = east;
            numSquaresToEdge[squareIndex][4] = Math.Min(north, west);
            numSquaresToEdge[squareIndex][5] = Math.Min(south, east);
            numSquaresToEdge[squareIndex][6] = Math.Min(north, east);
            numSquaresToEdge[squareIndex][7] = Math.Min(south, west);

            //Debug.Log(squareIndex + " - " + string.Join(",", numSquaresToEdge[squareIndex]));
        }

        
       // Debug.Log("Move data done!");

    }

    private void SpawnObstacles()
    {
        for (int i = 0; i < NumObstacles; i++)
        {
            //GridCell cell;
            int randX, randY;
            do
            {
                //todo : set spawn area where obstacles cannot be spawned...
                randX = Random.Range(0, Width);
                randY = Random.Range(0, Height);
                //cell = GetCell(randX, randY);
            } while (IsCellOccupied(randX, randY));

            GridCell cell = GetCell(randX, randY);

            Vector3 cellPos = GetGridCellWorldPosition(cell);
            _ = Instantiate(ObstaclePrefab, cellPos, Quaternion.identity);
            

            Debug.Log($"Obstacle added at x:{randX}, y:{randY}");

        }
    }

    public void AddCreaturePosition(Creature creature)
    {
        //GridCell cell = GetCell(creature.CellX, creature.CellY);
        Vector2 cellCoords = new Vector2(creature.CellX, creature.CellY);

        if (creature.IsFriendly)
        {
            PlayerCharacterPositions.Add(creature.ID, cellCoords);
        }
        else
        {
            EnemyPositions.Add(creature.ID, cellCoords);
        }
    }

    public void UpdateCreaturePosition(Creature creature)
    {
        Vector2 cellCoords = new Vector2(creature.CellX, creature.CellY);

        if (creature.IsFriendly)
        {
            PlayerCharacterPositions[creature.ID] = cellCoords;
        }
        else
        {
            EnemyPositions[creature.ID] = cellCoords;
        }

    }



    private bool IsCellOccupied(CreatureRuntimeSet creatures, int x, int y)
    {
        return creatures.Items.Where(w => w.CellX == x && w.CellY == y).Any();
    }




    private void CheckCellOccupied(CreatureRuntimeSet creatures, GridCell cell, ActionClass action, int x, int y)
    {
        List<GridCell> attackedCells = new List<GridCell>();

        attackedCells.Add(cell);
        int origX = cell.X;
        int origY = cell.Y;

        foreach (Vector2 offset in action.additionalAttackedCellOffsets)
        {
            int newX = origX + (int)offset.x;
            int newY = origY + (int)offset.y;

            if (CellExists(newX, newY))
            {
                attackedCells.Add(GetCell(newX, newY));
            }
        }



        var creature = creatures.Items.Where(w => w.CellX == x && w.CellY == y).SingleOrDefault();

        if (creature != null)
        {
            _targetCells.Add(cell);
            _targets.Add(creature);
        }
    }

    private EnemyActionResult GetActionResult(CreatureRuntimeSet creatures, GridCell targetCell, ActionClass action, int x, int y)
    {
        EnemyActionResult result = new EnemyActionResult(targetCell, action);
        List<GridCell> attackedCells = new List<GridCell>();

        attackedCells.Add(targetCell);
        int origX = targetCell.X;
        int origY = targetCell.Y;

        foreach (Vector2 offset in action.additionalAttackedCellOffsets)
        {
            int newX = origX + (int)offset.x;
            int newY = origY + (int)offset.y;

            if (CellExists(newX, newY))
            {
                attackedCells.Add(GetCell(newX, newY));
            }
        }

        foreach (GridCell cell in attackedCells)
        {
            var creature = creatures.Items.Where(w => w.CellX == x && w.CellY == y).SingleOrDefault();

            if (creature != null)
            {
                result.Creatures.Add(creature);

                _targetCells.Add(cell);
                _targets.Add(creature);
            }
        }

        if (result.Creatures.Count > 0 )
        {
            return result;
        }

        return null;
    }


    private bool IsCellOccupied(int x, int y)
    {
        return _combatants.Items.Where(w => w.CellX == x && w.CellY == y).Any();
    }

    private Creature GetCellOccupant(int x, int y)
    {
        return _combatants.Items.Where(w => w.CellX == x && w.CellY == y).SingleOrDefault();
    }


    public List<GridCell> GetMoves(MoveClass moveClass, int x, int y)
    {
        List<GridCell> moves = new List<GridCell>();

        int startDirIndex = 0;
        int endDirIndex = 8;

        if (moveClass.IsJumpingPiece)
        {
            if (moveClass.MoveType == MoveTypeEnum.Knight)
            {
                foreach (var jumpMove in knightOffsets)
                {
                    int squareX = x + jumpMove.Item1;
                    int squareY = y + jumpMove.Item2;
                    if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
                    {
                        if (!IsCellOccupied(squareX, squareY))
                        {
                            GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                            moves.Add(cell);
                        }
                    }
                }
            }
        }
        else
        {
            int playerSquare = (y * Height) + x;

            switch (moveClass.MoveType)
            {
                case MoveTypeEnum.Queen:
                    Debug.Log("King/Queen - consider all directions!");
                    break;
                case MoveTypeEnum.Bishop:
                    startDirIndex = 4;
                    break;
                case MoveTypeEnum.Rook:
                    endDirIndex = 4;
                    break;
                default:
                    Debug.LogFormat("Unrecognised move type, defaulting to all directions!");
                    break;
            }

            for (int dirIndex = startDirIndex; dirIndex < endDirIndex; dirIndex++)
            {
                var currentDirOffset = slideOffsets[dirIndex];

                int maxDistance = Math.Min(moveClass.MoveLimit, numSquaresToEdge[playerSquare][dirIndex]);

                for (int n = 0; n < maxDistance; n++)
                {
                    int squareX = x + currentDirOffset.Item1 * (n + 1);
                    int squareY = y + currentDirOffset.Item2 * (n + 1);

                    if (IsCellOccupied(squareX, squareY))
                    {
                        break;
                    }

                    GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                    moves.Add(cell);                    
                }

            }
        }

        return moves;
    }


    //public List<Vector2> GetMovesNew(MoveClass moveClass, int x, int y)
    //{
    //    List<Vector2> moves = new List<Vector2>();

    //    int startDirIndex = 0;
    //    int endDirIndex = 8;

    //    if (moveClass.IsJumpingPiece)
    //    {
    //        if (moveClass.MoveType == MoveTypeEnum.Knight)
    //        {
    //            foreach (var jumpMove in knightOffsets)
    //            {
    //                int squareX = x + jumpMove.Item1;
    //                int squareY = y + jumpMove.Item2;
    //                if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
    //                {
    //                    if (!IsCellOccupied(squareX, squareY))
    //                    {
    //                        moves.Add(new Vector2(squareX, squareY));
    //                        //GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                        //moves.Add(cell);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        int playerSquare = (y * Height) + x;

    //        switch (moveClass.MoveType)
    //        {
    //            case MoveTypeEnum.Queen:
    //                Debug.Log("King/Queen - consider all directions!");
    //                break;
    //            case MoveTypeEnum.Bishop:
    //                startDirIndex = 4;
    //                break;
    //            case MoveTypeEnum.Rook:
    //                endDirIndex = 4;
    //                break;
    //            default:
    //                Debug.LogFormat("Unrecognised move type, defaulting to all directions!");
    //                break;
    //        }

    //        for (int dirIndex = startDirIndex; dirIndex < endDirIndex; dirIndex++)
    //        {
    //            var currentDirOffset = slideOffsets[dirIndex];

    //            int maxDistance = Math.Min(moveClass.MoveLimit, numSquaresToEdge[playerSquare][dirIndex]);

    //            for (int n = 0; n < maxDistance; n++)
    //            {
    //                int squareX = x + currentDirOffset.Item1 * (n + 1);
    //                int squareY = y + currentDirOffset.Item2 * (n + 1);

    //                if (IsCellOccupied(squareX, squareY))
    //                {
    //                    break;
    //                }

    //                moves.Add(new Vector2(squareX, squareY));
    //                //GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                //moves.Add(cell);
    //            }

    //        }
    //    }

    //    return moves;
    //}



    public void ShowMovesForPlayer(MoveClass moveClass, int playerX, int playerY)
    {
        //ClearGrid();
        List<GridCell> moves = GetMoves(moveClass, playerX, playerY);



        moves.ForEach(f => f.SetAsValidMove());

    }

    //public void ShowMovesForPlayerNew(MoveClass moveClass, int x, int y)
    //{
    //    List<Vector2> moves = GetMovesNew(moveClass, x, y);

    //    foreach (Vector2 move in moves)
    //    {
    //        int moveX = (int)move.x;
    //        int moveY = (int)move.y;
    //        Grid[moveX, moveY].gameObject.SetActive(true);
    //        GridCell cell = Grid[moveX, moveY].GetComponent<GridCell>();
    //        cell.SetAsValidMove();
    //    }

    //}

    public void GetMovesForPlayer(MoveClass moveClass, int playerX, int playerY)
    {
        ClearGrid();
        Debug.Log("Hero located x:" + playerX + ", y:" + playerY);

        int startDirIndex = 0;
        int endDirIndex = 8;

        if (moveClass.IsJumpingPiece)
        {
            if (moveClass.MoveType == MoveTypeEnum.Knight)
            {
                foreach (var jumpMove in knightOffsets)
                {
                    int squareX = playerX + jumpMove.Item1;
                    int squareY = playerY + jumpMove.Item2;
                    if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
                    {
                        if (!IsCellOccupied(squareX, squareY))
                        {
                            GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                            cell.SetAsValidMove();
                        }

                        //GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                        //if (!cell.IsOccupiedNew())
                        //{
                        //    cell.SetAsValidMove();
                        //}
                    }
                }
            }
        }
        else
        {
            int playerSquare = (playerY * Height) + playerX;
            Debug.Log("Character is in: " + playerSquare);

            switch (moveClass.MoveType)
            {
                case MoveTypeEnum.Queen:
                    Debug.Log("King/Queen - consider all directions!");
                    break;
                case MoveTypeEnum.Bishop:
                    startDirIndex = 4;
                    break;
                case MoveTypeEnum.Rook:
                    endDirIndex = 4;
                    break;
                default:
                    Debug.LogFormat("Unrecognised move type, defaulting to all directions!");
                    break;
            }

            for (int dirIndex = startDirIndex; dirIndex < endDirIndex; dirIndex++)
            {
                var currentDirOffset = slideOffsets[dirIndex];

                int maxDistance = Math.Min(moveClass.MoveLimit, numSquaresToEdge[playerSquare][dirIndex]);

                for (int n = 0; n < maxDistance; n++)
                {
                    //Debug.Log(currentDirOffset + " " + (n + 1));
                    int squareX = playerX + currentDirOffset.Item1 * (n + 1);
                    int squareY = playerY + currentDirOffset.Item2 * (n + 1);
                    Debug.Log(squareX + ", " + squareY);

                    if (IsCellOccupied(squareX, squareY))
                    {
                        break;
                    }

                    GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                    cell.SetAsValidMove();
                    

                }

            }
        }


    }




    private int CalculateChebyshevDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
    }


    public List<EnemyActionResult> GetTargetsOfActionNew(ActionClass action, CreatureRuntimeSet targetCreatures, int x, int y)
    {
        List<GridCell> targets = new List<GridCell>();
        List<EnemyActionResult> results = new List<EnemyActionResult>();
        //_actionResults = new List<ActionResult>();
        
        _targetCells.Empty();

        if (action.IsRanged)
        {

            int minRange = action.MinRange;
            int maxRange = action.MaxRange;


            for (int i = x - maxRange; i <= x + maxRange; i++)
            {
                for (int j = y - maxRange; j <= y + maxRange; j++)
                {
                    if (0 <= i && i < Width && 0 <= j && j < Height)
                    {

                        GridCell cell = Grid[i, j].GetComponent<GridCell>();
                        int cellChebyshevDistance = CalculateChebyshevDistance(x, i, y, j);

                        if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange)
                        {
                            //CheckCellOccupied(targetCreatures, cell, action, i, j);
                            var result = GetActionResult(targetCreatures, cell, action, i, j);
                            if (result != null)
                            {
                                results.Add(result);
                            }
                        }
                    }

                }
            }
        }
        else // melee
        {
            // Check only the cells immediately adjacent to the active character
            // only set a cell as a valid attack if it is occupied.
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }

                    if (0 <= i && i < Width && 0 <= j && j < Height)
                    {
                        GridCell cell = Grid[i, j].GetComponent<GridCell>();

                        var result = GetActionResult(targetCreatures, cell, action, i, j);
                        if (result != null)
                        {
                            results.Add(result);
                        }
                    }
                }
            }

        }

        return results;

    }



    //public List<GridCell> GetTargetsOfAction(ActionClass action, int x, int y)
    //{
    //    List<GridCell> targets = new List<GridCell>();

    //    if (action.IsRanged)
    //    {

    //        int minRange = action.MinRange;
    //        int maxRange = action.MaxRange;


    //        for (int i = x - maxRange; i <= x + maxRange; i++)
    //        {
    //            for (int j = y - maxRange; j <= y + maxRange; j++)
    //            {
    //                if (0 <= i && i < Width && 0 <= j && j < Height)
    //                {

    //                    GridCell cell = Grid[i, j].GetComponent<GridCell>();
    //                    int cellChebyshevDistance = CalculateChebyshevDistance(x, i, y, j);

    //                    if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange)
    //                    {
    //                        if (IsCellOccupied(cell))
    //                        {
    //                            //cell.SetAsValidAttack();
    //                            targets.Add(cell);
    //                        }
                            
    //                    }
    //                }

    //            }
    //        }
    //    }
    //    else // melee
    //    {
    //        // Check only the cells immediately adjacent to the active character
    //        // only set a cell as a valid attack if it is occupied.
    //        for (int i = x - 1; i <= x + 1; i++)
    //        {
    //            for (int j = y - 1; j <= y + 1; j++)
    //            {
    //                if (i == x && j == y)
    //                {
    //                    continue;
    //                }

    //                if (0 <= i && i < Width && 0 <= j && j < Height)
    //                {
    //                    GridCell cell = Grid[i, j].GetComponent<GridCell>();
    //                    if (IsCellOccupied(cell))
    //                    {
    //                        //cell.SetAsValidAttack();
    //                        targets.Add(cell);
    //                    }


    //                    //int cellChebyshevDistance = CalculateChebyshevDistance(x, i, y, j);
    //                    ////Debug.Log($"({i},{j}) = {cellChebyshevDistance}");

    //                    //if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange)
    //                    //{
    //                    //    cell.SetAsValidAttack();
    //                    //}
    //                }
    //            }
    //        }

    //    }

    //    return targets;
    //}



    public List<Creature> GetAttackedCreatures(GridCell targetCell, ActionClass action)
    {
        List<Creature> attackedCreatures = new List<Creature>();
        List<GridCell> attackedCells = new List<GridCell>();

        // get attacked cells by attack
        attackedCells.Add(targetCell);
        int origX = targetCell.X;
        int origY = targetCell.Y;

        foreach (Vector2 offset in action.additionalAttackedCellOffsets)
        {
            int newX = origX + (int)offset.x;
            int newY = origY + (int)offset.y;

            if (CellExists(newX, newY))
            {
                attackedCells.Add(GetCell(newX, newY));
            }
        }

        // check each of these cells for creatures
        foreach (GridCell cell in attackedCells)
        {
            var creature = GetCellOccupant(cell.X, cell.Y);
            if (creature != null)
            {
                attackedCreatures.Add(creature);
            }

            //var creatureID = GetCellOccupant(cell);
            //if (creatureID.HasValue)
            //{
            //    attackedCreatures.Add(creatureID.Value);
            //}

            //if (IsCellOccupied(cell))
            //{
            //    // Need more than just the id!
            //    //attackedCreatures.Add()

            //}

            //if (cell.OccupiedUnit != null)
            //{
            //   attackedCreatures.Add(cell.OccupiedUnit);
            //}    
        }

        return attackedCreatures;
    }


    public void ClearGrid()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                //Grid[x, y].gameObject.SetActive(false);
                GridCell cell = GetCell(x, y);
                //cell.SetMoveText("");
                cell.ResetCell();
            }
        }
    }


    public Vector3 GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / GridSpacesize);
        int y = Mathf.FloorToInt(worldPosition.y / GridSpacesize);

        x = Mathf.Clamp(x, 0, Width);
        y = Mathf.Clamp(y, 0, Height);

        return new Vector3(x, 0.1f, y);

    }


    public Vector3 GetWorldPosFromGridPos(Vector3 gridPos)
    {
        float x = gridPos.x * GridSpacesize;
        float y = gridPos.y * GridSpacesize;

        return new Vector3(x, 0.1f, y);

    }


    public Vector3 GetGridCellWorldPosition(GridCell cell)
    {
        Vector3 cellPos = cell.GetPosition();
        float x = cellPos.x * GridSpacesize;
        float z = cellPos.z * GridSpacesize;

        return new Vector3(x, 0.1f, z);
    }


    public GridCell GetCell(int x, int y)
    {
        return Grid[x, y].GetComponent<GridCell>();
    }


    public GridCell GetRandomCell()
    {
        int x = Random.Range(0, Width);
        int y = Random.Range(0, Height);

        return GetCell(x, y);
    }


    public GridCell GetRandomUnoccupiedCell()
    {
        
        int randX, randY;
        do
        {
            //todo : set spawn area where obstacles cannot be spawned...
            randX = Random.Range(0, Width);
            randY = Random.Range(0, Height);
        } while (IsCellOccupied(randX, randY));

        GridCell cell = GetCell(randX, randY);

        return cell;
    }


    private bool CellExists(int x, int y)
    {
        return ((0 <= x) && (x < Height) && (0 <= y) && (y < Width));
    }

    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << 6;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layerMask))
        {
            GameObject hitObject = hitInfo.transform.gameObject;
            if (hitObject.TryGetComponent(out GridCell selectedCell))
            {
                if (selectedCell.IsSelectable)
                {
                   // Debug.Log(selectedCell.name, this);

                    if (Input.GetMouseButtonDown(0))
                    {
                        SelectCell(selectedCell);
                        return;
                    }

                    HighlightCell(selectedCell);


                    //if (State == BattleStatesEnum.PLAYER_MOVE)
                    //{
                    //    BattleEvents.CellMoveSelected(selectedCell);
                    //}
                    //else
                    //{
                    //    BattleEvents.CellAttackSelected(selectedCell);
                    //}
                }
                else
                {
                    UnhighlightCell(selectedCell);
                }
            }
        }


        //if (State == BattleStatesEnum.PLAYER_MOVE || State == BattleStatesEnum.PLAYER_ATTACK)
        //{
        //if (Input.GetMouseButtonDown(0))
        //{
            

        //    //if (!EventSystem.current.IsPointerOverGameObject())
        //    //{
            
        //}
        //}
    }



}
