using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameGrid : MonoBehaviour
{
    public readonly int Height = 10;
    public readonly int Width = 10;
    private float GridSpacesize = 5f;

    private int NumObstacles;

    [SerializeField]
    private GameObject GridCellPrefab;
    private GameObject ObstaclePrefab;
    private GameObject[,] Grid;

    [SerializeField]
    private Color lightColor;
    [SerializeField]
    private Color darkColor;

    private BattleSystem BattleSystem;


    private int[][] numSquaresToEdge;
    private List<(int, int)> slideOffsets;
    private List<(int, int)> knightOffsets;

    private List<(int, int)> baseAttack = new List<(int, int)>() { (1, 0), (-1, 0), (0, 1), (0, -1) };
    private List<(int, int)> _cellNeighbourDirections = new() { (1, 0), (-1, 0), (0, 1), (0, -1) };



    public void Awake()
    {

        GridCellPrefab = Resources.Load("Prefabs/GridCell") as GameObject;
        ObstaclePrefab = Resources.Load("Prefabs/Obstacle") as GameObject;

    }

    public IEnumerator CreateGameGridCoroutine(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
        Grid = new GameObject[Height, Width];

        yield return new WaitForSeconds(1f);

        if (GridCellPrefab == null)
        {
            Debug.Log("ERROR: Grid cell prefab not set!");
        }

        // Make Grid
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Grid[x, y] = Instantiate(GridCellPrefab, new Vector3(x * GridSpacesize, 0.1f, y * GridSpacesize), Quaternion.identity);

                bool isLightSquare = (x + y) % 2 != 0;
                var squareColour = isLightSquare ? lightColor : darkColor;
                GridCell cell = GetCell(x, y);
                cell.Setup(BattleSystem, x, y, squareColour);
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
        knightOffsets = new List<(int, int)>() { (2, 1), (1, 2), (-1, 2), (-2, 1), (-2, -1), (-1, -2), (1, -2), (2, -1) };

        int totalSquares = Width * Height;
        numSquaresToEdge = new int[totalSquares][];

        // First 4 are orthogonal, last 4 are diagonals (N, S, W, E, NW, SE, NE, SW)
        for (int squareIndex = 0; squareIndex < totalSquares; squareIndex++)
        {
            int y = squareIndex / Width;
            int x = squareIndex - y * Width;

            int north = Height - y - 1;
            int south = y;
            int west = x;
            int east = Width - x - 1;
            numSquaresToEdge[squareIndex] = new int[8];
            numSquaresToEdge[squareIndex][0] = north;
            numSquaresToEdge[squareIndex][1] = south;
            numSquaresToEdge[squareIndex][2] = west;
            numSquaresToEdge[squareIndex][3] = east;
            numSquaresToEdge[squareIndex][4] = System.Math.Min(north, west);
            numSquaresToEdge[squareIndex][5] = System.Math.Min(south, east);
            numSquaresToEdge[squareIndex][6] = System.Math.Min(north, east);
            numSquaresToEdge[squareIndex][7] = System.Math.Min(south, west);

        }

        Debug.Log("Move data done!");

    }

    private void SpawnObstacles()
    {
        for (int i = 0; i < NumObstacles; i++)
        {
            GridCell cell;
            int randX, randY;
            do
            {
                //todo : set spawn area where obstacles cannot be spawned...
                randX = Random.Range(0, Width);
                randY = Random.Range(0, Height);
                cell = GetCell(randX, randY);
            } while (cell.IsOccupiedNew());

            Vector3 cellPos = GetGridCellWorldPosition(cell);
            _ = Instantiate(ObstaclePrefab, cellPos, Quaternion.identity);
            

            Debug.Log($"Obstacle added at x:{randX}, y:{randY}");

        }
    }

    public void GetMovesForPlayerNew(MoveClass moveClass, GridCell playerCell)
    {
        ClearGrid();
        Debug.Log("Hero located x:" + playerCell.X + ", y:" + playerCell.Y);

        int startDirIndex = 0;
        int endDirIndex = 8;

        if (moveClass.IsJumpingPiece)
        {
            if (moveClass.MoveType == MoveTypeEnum.Knight)
            {
                foreach (var jumpMove in knightOffsets)
                {
                    int squareX = playerCell.X + jumpMove.Item1;
                    int squareY = playerCell.Y + jumpMove.Item2;
                    if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
                    {
                        GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                        if (!cell.IsOccupiedNew())
                        {
                            cell.SetAsValidMove();
                        }
                    }
                }
            }
        }
        else
        {
            int playerSquare = (playerCell.Y * Width) + playerCell.X;

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
                    Debug.Log(currentDirOffset + " " + (n + 1));
                    int squareX = playerCell.X + currentDirOffset.Item1 * (n + 1);
                    int squareY = playerCell.Y + currentDirOffset.Item2 * (n + 1);
                    GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                    if (cell.IsOccupiedNew())
                    {
                        break;
                    }

                    cell.SetAsValidMove();
                }

            }
        }


    }


    public void GetMovesForPlayer(MoveClass moveClass, int x, int y)
    {
        ClearGrid();
        Debug.Log("Hero located x:" + x + ", y:" + y);

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
                        GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                        if (!cell.IsOccupiedNew())
                        {
                            cell.SetAsValidMove();
                        }
                    }
                }
            }
        }
        else
        {
            int playerSquare = (y * Width) + x;

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
                    Debug.Log(currentDirOffset + " " + (n + 1));
                    int squareX = x + currentDirOffset.Item1 * (n + 1);
                    int squareY = y + currentDirOffset.Item2 * (n + 1);
                    GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                    if (cell.IsOccupiedNew())
                    {
                        break;
                    }

                    cell.SetAsValidMove();
                }

            }
        }


    }

    public List<GridCell> GetBaseAttack(GridCell playerCell)
    {
        ClearGrid();
        List<GridCell> AttackCells = new();

        foreach (var attack in baseAttack)
        {
            int squareX = playerCell.X + attack.Item1;
            int squareY = playerCell.Y + attack.Item2;
            if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
            {
                GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
               // Debug.Log(Vector3.Distance(GetGridCellWorldPosition(cell), GetGridCellWorldPosition(playerCell)));

                if (cell.IsOccupiedNew())
                {
                    cell.SetAsValidAttack();
                    AttackCells.Add(cell);
                }
            }
        }

        return AttackCells;
    }

    public List<GridCell> GetFireballAttack(GridCell playerCell)
    {
        ClearGrid();
        List<GridCell> AttackCells = new();

        Vector3 playerPos = GetGridCellWorldPosition(playerCell);

        Debug.Log($"Fireball {playerCell.X},{playerCell.Y}");

        int minRange = 2;
        int maxRange = 3;

        for (int i = playerCell.X - maxRange; i <= playerCell.X + maxRange; i++)
        {
            for (int j = playerCell.Y - maxRange; j <= playerCell.Y + maxRange; j++)
            {
                if (0 <= i && i < Width && 0 <= j && j < Height)
                {

                    GridCell cell = Grid[i, j].GetComponent<GridCell>();
                    int cellChebyshevDistance = CalculateChebyshevDistance(playerCell.X, i, playerCell.Y, j);
                    Debug.Log($"({i},{j}) = {cellChebyshevDistance}");

                    if (cellChebyshevDistance >= minRange && cellChebyshevDistance <= maxRange && !cell.IsOccupiedNew())
                    {
                        cell.SetAsValidAttack();
                        AttackCells.Add(cell);
                    }                  
                }

            }
        }

        return AttackCells;
    }

    private int CalculateChebyshevDistance(int x1, int x2, int y1, int y2)
    {
        return Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
    }

    public List<GridCell> GetAttacksForPlayer(int x, int y)
    {
        ClearGrid();
        List<GridCell> AttackCells = new List<GridCell>();

        foreach (var attack in baseAttack)
        {
            int squareX = x + attack.Item1;
            int squareY = y + attack.Item2;
            if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
            {
                GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                if (cell.IsOccupiedNew())
                {
                    cell.SetAsValidAttack();
                    AttackCells.Add(cell);
                }
            }
        }

        return AttackCells;
    }

    public List<GridCell> TestAttacksForPlayer(GridCell playerCell, int range)
    {
        List<GridCell> AttackCells = new List<GridCell>();

        foreach (var neighbourDirection in _cellNeighbourDirections)
        {
            int squareX = playerCell.X + neighbourDirection.Item1;
            int squareY = playerCell.Y + neighbourDirection.Item2;
            if (0 <= squareX && squareX < Width && 0 <= squareY && squareY < Height)
            {
                GridCell cell = Grid[squareX, squareY].GetComponent<GridCell>();
                if (!AttackCells.Contains(cell))
                {
                    if (cell.IsOccupiedNew())
                    {
                        cell.SetAsValidAttack();
                        AttackCells.Add(cell);
                    }
                }                
            }
        }

        return AttackCells;
    }


    //public void UpdateGridOfMove(int startX, int startY, int finishX, int finishY)
    //{
    //    GetCell(startX, startY).IsOccupied = false;
    //    GetCell(finishX, finishY).IsOccupied = true;
    //}

    public void ClearGrid()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
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
        GridCell cell;
        int randX, randY;
        do
        {
            //todo : set spawn area where obstacles cannot be spawned...
            randX = Random.Range(0, Width);
            randY = Random.Range(0, Height);
            cell = GetCell(randX, randY);
        } while (cell.OccupiedUnit != null);

        return cell;
    }

    public void AddToCell(Creature c, int x, int y)
    {
        GridCell cell = GetCell(x, y);

    }

    private bool CellExists(int x, int y)
    {
        return ((0 <= x) && (x <= Height) && (0 <= y) && (y <= Height));
    }

}
