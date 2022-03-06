using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testbed : MonoBehaviour
{

    [SerializeField] private GameObject GridCellPrefab;
    [SerializeField] private int Height;
    [SerializeField] private int Width;

    [SerializeField] private Color lightColor;
    [SerializeField] private Color darkColor;

    private GameObject[,] Grid;
    private float GridSpacesize = 5f;

    // Start is called before the first frame update
    void Start()
    {
        CreateTestGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void CreateTestGrid()
    {

        Grid = new GameObject[Height, Width];

        // Make Grid
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Grid[x, y] = Instantiate(GridCellPrefab, new Vector3(x * GridSpacesize, 0.1f, y * GridSpacesize), Quaternion.identity);

                bool isLightSquare = (x + y) % 2 != 0;
                var squareColour = isLightSquare ? lightColor : darkColor;
                //GridCell cell = GetCell(x, y);
                //cell.Setup(BattleSystem, x, y, squareColour);
                Grid[x,y].transform.parent = transform;
                Grid[x,y].name = $"Grid Space (x:{x}, y:{y})";
            }
        }

    }

}
