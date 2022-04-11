using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameGrid : Singleton<GameGrid>
{
    public readonly int Height = 10;
    public readonly int Width = 10;
    private float GridSpacesize = 5f;

    private int NumObstacles;

    [SerializeField] private GameObject GridCellPrefab;
    private GameObject ObstaclePrefab;
    private GameObject[,] Grid;

    [SerializeField] private Color lightColor;
    [SerializeField] private Color darkColor;

    [SerializeField] private GameObject _attackTemplatePrefab;

    private BattleSystem BattleSystem;


    private int[][] numSquaresToEdge;
    private List<(int, int)> slideOffsets;
    private List<(int, int)> knightOffsets;

    private List<(int, int)> baseAttack = new List<(int, int)>() { (1, 0), (-1, 0), (0, 1), (0, -1) };
    private List<(int, int)> _cellNeighbourDirections = new() { (1, 0), (-1, 0), (0, 1), (0, -1) };


    public Dictionary<int, Vector2> PlayerCharacterPositions { get; private set; }
    public Dictionary<int, Vector2> EnemyPositions { get; private set; }


    public void Awake()
    {
        PlayerCharacterPositions = new Dictionary<int, Vector2>();
        EnemyPositions = new Dictionary<int, Vector2>();

        GridCellPrefab = Resources.Load("Prefabs/NewGridCell") as GameObject;
        ObstaclePrefab = Resources.Load("Prefabs/Obstacle") as GameObject;

        BattleEvents.OnPlayerActionSelected += ShowActionOnGrid;
        BattleEvents.OnCreatureMoved += UpdateCreaturePosition;
        //BattleEvents.OnTurnOver += ClearGrid;
    }


    private void OnDisable()
    {
        BattleEvents.OnPlayerActionSelected -= ShowActionOnGrid;
        BattleEvents.OnCreatureMoved -= UpdateCreaturePosition;
        //BattleEvents.OnTurnOver -= ClearGrid;
    }


    private void ShowActionOnGrid(int characterID, ActionClass action, int x, int y)
    {

        ClearGrid();

        // get attack template from action
        Debug.Log("Action: " + action.Name + " selected!");

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
                    //Debug.Log($"({i},{j}) = {cellChebyshevDistance}");

                    if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange)
                    {
                        cell.SetAsValidAttack();
                    }
                }

            }
        }

    }

    public void CreateGameGrid(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
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

                Grid[x, y] = Instantiate(GridCellPrefab, cellPosition, Quaternion.Euler(90, 0, 0));

                bool isLightSquare = (x + y) % 2 != 0;
                var squareColour = isLightSquare ? lightColor : darkColor;
                GridCell cell = GetCell(x, y);
                cell.Setup(x, y, cellPosition, squareColour);
                cell.transform.parent = transform;
                cell.name = $"Grid Space (x:{x}, y:{y})";
            }
        }

        ComputeMoveData();
        SpawnObstacles();


    }

    public IEnumerator CreateGameGridCoroutine(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
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

                Grid[x, y] = Instantiate(GridCellPrefab, cellPosition, Quaternion.Euler(90, 0, 0));

                bool isLightSquare = (x + y) % 2 != 0;
                var squareColour = isLightSquare ? lightColor : darkColor;
                GridCell cell = GetCell(x, y);
                cell.Setup(x, y, cellPosition, squareColour);
                cell.transform.parent = transform;
                cell.name = $"Grid Space (x:{x}, y:{y})";
            }
        }

        yield return new WaitForSeconds(0.1f);

        ComputeMoveData();
        SpawnObstacles();
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



    private bool IsCellOccupied(GridCell cell)
    {

        Vector2 cellCoords = new Vector2(cell.X, cell.Y);

        if (PlayerCharacterPositions.ContainsValue(cellCoords))
        {
            return true;
        }
        else if (EnemyPositions.ContainsValue(cellCoords))
        {
            return true;
        }

        return false;
    }

    private bool IsCellOccupied(int cellX, int cellY)
    {

        Vector2 cellCoords = new Vector2(cellX, cellY);

        if (PlayerCharacterPositions.ContainsValue(cellCoords))
        {
            return true;
        }
        else if (EnemyPositions.ContainsValue(cellCoords))
        {
            return true;
        }

        return false;
    }

    private int? GetCellOccupant(GridCell cell)
    {

        Vector2 cellCoords = new Vector2(cell.X, cell.Y);

        foreach (KeyValuePair<int,Vector2> kvp in EnemyPositions)
        {
            if (kvp.Value == cellCoords)
            {
                return kvp.Key;
            }
        }

        foreach (KeyValuePair<int, Vector2> kvp in PlayerCharacterPositions)
        {
            if (kvp.Value == cellCoords)
            {
                return kvp.Key;
            }
        }

        return null;
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


    public void ShowMovesForPlayer(MoveClass moveClass, int playerX, int playerY)
    {
        //ClearGrid();
        List<GridCell> moves = GetMoves(moveClass, playerX, playerY);

        moves.ForEach(f => f.SetAsValidMove());

    }

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
                    

                    
                    //if (cell.IsOccupiedNew())
                    //{
                    //    break;
                    //}

                    //cell.SetAsValidMove();
                    //cell.gameObject.SetActive(true);
                }

            }
        }


    }




    //public void GetMovesForPlayer(MoveClass moveClass, int x, int y)
    //{
    //    ClearGrid();
    //    Debug.Log("Hero located x:" + x + ", y:" + y);

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
    //                        GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                        cell.SetAsValidMove();
    //                    }

    //                    //GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                    //if (!cell.IsOccupiedNew())
    //                    //{
    //                    //    cell.SetAsValidMove();
    //                    //}
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        int playerSquare = (y * Width) + x;

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
    //                Debug.Log(currentDirOffset + " " + (n + 1));
    //                int squareX = x + currentDirOffset.Item1 * (n + 1);
    //                int squareY = y + currentDirOffset.Item2 * (n + 1);

    //                if (IsCellOccupied(squareX, squareY))
    //                {
    //                    break;
    //                }

    //                GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                cell.SetAsValidMove();
                    
    //                //GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                //if (cell.IsOccupiedNew())
    //                //{
    //                //    break;
    //                //}

    //                //cell.SetAsValidMove();
    //            }

    //        }
    //    }


    //}

    //public List<GridCell> GetBaseAttack(GridCell playerCell)
    //{
    //    ClearGrid();
    //    List<GridCell> AttackCells = new();

    //    foreach (var attack in baseAttack)
    //    {
    //        int squareX = playerCell.X + attack.Item1;
    //        int squareY = playerCell.Y + attack.Item2;
    //        if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
    //        {
    //            //GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //            // Debug.Log(Vector3.Distance(GetGridCellWorldPosition(cell), GetGridCellWorldPosition(playerCell)));

    //            if (!IsCellOccupied(squareX, squareY))
    //            {
    //                GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //                cell.SetAsValidAttack();
    //                AttackCells.Add(cell);
    //            }

    //            //if (cell.IsOccupiedNew())
    //            //{
    //            //    cell.SetAsValidAttack();
    //            //    AttackCells.Add(cell);
    //            //}
    //        }
    //    }

    //    return AttackCells;
    //}

    //public List<GridCell> GetFireballAttack(GridCell playerCell)
    //{
    //    ClearGrid();
    //    List<GridCell> AttackCells = new();

    //    Vector3 playerPos = GetGridCellWorldPosition(playerCell);

    //    Debug.Log($"Fireball {playerCell.X},{playerCell.Y}");

    //    int minRange = 2;
    //    int maxRange = 3;

    //    for (int i = playerCell.X - maxRange; i <= playerCell.X + maxRange; i++)
    //    {
    //        for (int j = playerCell.Y - maxRange; j <= playerCell.Y + maxRange; j++)
    //        {
    //            if (0 <= i && i < Width && 0 <= j && j < Height)
    //            {

    //                GridCell cell = Grid[i, j].GetComponent<GridCell>();
    //                int cellChebyshevDistance = CalculateChebyshevDistance(playerCell.X, i, playerCell.Y, j);
    //                Debug.Log($"({i},{j}) = {cellChebyshevDistance}");

    //                if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange && !cell.IsOccupiedNew())
    //                {
    //                    cell.SetAsValidAttack();
    //                    AttackCells.Add(cell);
    //                }                  
    //            }

    //        }
    //    }

    //    return AttackCells;
    //}

    private int CalculateChebyshevDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
    }

    //public List<GridCell> GetAttacksForPlayer(int x, int y)
    //{
    //    ClearGrid();
    //    List<GridCell> AttackCells = new List<GridCell>();

    //    foreach (var attack in baseAttack)
    //    {
    //        int squareX = x + attack.Item1;
    //        int squareY = y + attack.Item2;
    //        if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
    //        {
    //            GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //            if (cell.IsOccupiedNew())
    //            {
    //                cell.SetAsValidAttack();
    //                AttackCells.Add(cell);
    //            }
    //        }
    //    }

    //    return AttackCells;
    //}

    //public List<GridCell> TestAttacksForPlayer(GridCell playerCell, int range)
    //{
    //    List<GridCell> AttackCells = new List<GridCell>();

    //    foreach (var neighbourDirection in _cellNeighbourDirections)
    //    {
    //        int squareX = playerCell.X + neighbourDirection.Item1;
    //        int squareY = playerCell.Y + neighbourDirection.Item2;
    //        if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
    //        {
    //            GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
    //            if (!AttackCells.Contains(cell))
    //            {
    //                if (cell.IsOccupiedNew())
    //                {
    //                    cell.SetAsValidAttack();
    //                    AttackCells.Add(cell);
    //                }
    //            }                
    //        }
    //    }

    //    return AttackCells;
    //}


    public List<int> GetAttackedCreatures(GridCell targetCell, ActionClass action)
    {
        List<int> attackedCreatures = new List<int>();
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
            var creatureID = GetCellOccupant(cell);
            if (creatureID.HasValue)
            {
                attackedCreatures.Add(creatureID.Value);
            }

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
                GridCell cell = GetCell(x, y);
                //cell.SetMoveText("");
                cell.ResetCell();
                //cell.gameObject.SetActive(false);
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

    //public void AddToCell(Creature c, int x, int y)
    //{
    //    GridCell cell = GetCell(x, y);

    //}

    private bool CellExists(int x, int y)
    {
        return ((0 <= x) && (x < Height) && (0 <= y) && (y < Width));
    }

}
