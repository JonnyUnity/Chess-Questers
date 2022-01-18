using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    private int CellNumber;

    private Color initColour = Color.white;
    private Color colour = Color.white;
    [SerializeField]
    private Color highlightColour;

    // Saves a reference to the gameobject that gets placed on this cell.
    public GameObject objectInThisGridSpace = null;

    public bool IsOccupied;
    public bool IsMove;
    public bool IsAttack;

    private Renderer Renderer;

    private BattleSystem BattleSystem;

    [SerializeField]
    private TextMeshProUGUI CellNumberText;
    [SerializeField]
    private TextMeshProUGUI MoveText;

    public void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }

    public void Setup(BattleSystem battleSystem, int x, int y, Color color)
    {
        BattleSystem = battleSystem;
        X = x;
        Y = y;
        initColour = color;
        colour = color;

        UpdateCellColour(colour);
    }

    public void InitColour(Color newColour)
    {
        initColour = newColour;
        colour = initColour;
        UpdateCellColour(colour);
    }

    public void SetCellNumber(int num)
    {
        CellNumber = num;
        CellNumberText.text = num.ToString();
    }

    public void SetOccupied()
    {
        IsOccupied = true;
    }

    public void SetMoveText(string text)
    {
        MoveText.text = text;
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
        CellNumberText.text = x + "," + y;
    }

    
    public Vector3 GetPosition()
    {
        return new Vector3(X, 0.1f, Y);
    }


    public void OnMouseEnter()
    {
        if (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE && IsMove)
        {
            UpdateCellColour(highlightColour);
            BattleSystem.CharacterLook(X, Y);
        }

        //if (IsMove)
        //{
        //    UpdateCellColour(highlightColour);
        //}
    }

    public void OnMouseDown()
    {
        //if (IsMove && (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE || BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK))
        //{
        //    //UpdateCellColour(highlightColour);
        //    BattleSystem.OnGridSelection(X, Y);
        //}

        if ((IsMove && BattleSystem.State == BattleStatesEnum.PLAYER_MOVE) || (IsAttack && BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK))
        {
            BattleSystem.OnGridSelection(X, Y);
        }
    }

    public void OnMouseExit()
    {
        if (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE && IsMove)
        {
            UpdateCellColour(colour);
        }

        //if (IsMove)
        //{
        //    UpdateCellColour(colour);
        //}
    }

    private void UpdateCellColour(Color newColor)
    {
        Renderer.material.color = newColor;
    }


    public void SetAsValidMove()
    {
        IsMove = true;
        colour = Color.yellow;
        UpdateCellColour(colour);
    }

    public void SetAsValidAttack()
    {
        IsAttack = true;
        colour = Color.red;
        UpdateCellColour(colour);
    }

    public void ResetCell()
    {
        IsMove = false;
        IsAttack = false;
        colour = initColour;
        UpdateCellColour(colour);
    }

}
