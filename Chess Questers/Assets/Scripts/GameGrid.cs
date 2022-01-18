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


    public void Awake()
    {

        GridCellPrefab = Resources.Load("Prefabs/GridCell") as GameObject;
        ObstaclePrefab = Resources.Load("Prefabs/Obstacle") as GameObject;

        //lightColor = new Color(212, 212, 212, 0);
        //darkColor = new Color(26, 75, 140, 0);

    }

    public IEnumerator CreateGameGrid(BattleSystem battleSystem)
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
            } while (cell.IsOccupied);

            Vector3 cellPos = GetGridCellWorldPosition(cell);
            _ = Instantiate(ObstaclePrefab, cellPos, Quaternion.identity);
            cell.IsOccupied = true;

            Debug.Log($"Obstacle added at x:{randX}, y:{randY}");

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
                        if (!cell.IsOccupied)
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
                    if (cell.IsOccupied)
                    {
                        break;
                    }

                    cell.SetAsValidMove();
                }

            }
        }


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
                if (cell.IsOccupied)
                {
                    cell.SetAsValidAttack();
                    AttackCells.Add(cell);
                }
            }
        }

        return AttackCells;
    }

    public void UpdateGridOfMove(int startX, int startY, int finishX, int finishY)
    {
        GetCell(startX, startY).IsOccupied = false;
        GetCell(finishX, finishY).IsOccupied = true;
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
        } while (cell.IsOccupied);

        return cell;
    }

    private bool CellExists(int x, int y)
    {
        return ((0 <= x) && (x <= Height) && (0 <= y) && (y <= Height));
    }

}
