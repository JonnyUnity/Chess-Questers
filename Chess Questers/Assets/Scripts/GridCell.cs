using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector3 Position { get; private set; }

    private int CellNumber;

    private Color initColour = Color.white;
    private Color colour = Color.white;

    //[SerializeField] GameObject _highlight;

    [SerializeField] private Color _highlightColour;
    [SerializeField] private Color _moveColour;
    [SerializeField] private Color _attackColur;

    // Saves a reference to the gameobject that gets placed on this cell.
    //public Creature OccupiedUnit = null;

    //public bool IsOccupied;
    public bool IsMove;
    public bool IsAttack;

    private Renderer _renderer;
    private Renderer _highlightRenderer;
    private Material _highlightMaterial;

    //private BattleSystem BattleSystem;

    [SerializeField]
    private TextMeshProUGUI CellNumberText;
    [SerializeField]
    private TextMeshProUGUI MoveText;

    public bool IsSelectable => IsMove || IsAttack;


    public void Awake()
    {
        _renderer = GetComponent<Renderer>();
        //_highlightRenderer = _highlight.GetComponent<Renderer>();
        //_highlightMaterial = _highlight.GetComponent<Renderer>().GetComponent<Material>();
    }

    public void Setup(int x, int y, Vector3 position, Color color)
    {
        X = x;
        Y = y;
        Position = position;

        InitColour(color);

    }

    public void InitColour(Color newColour)
    {
        initColour = newColour;
        colour = initColour;
        UpdateCellColour(colour);
    }


    //public void SetUnit(Creature c)
    //{
    //    //if (c.OccupiedCell != null)
    //    //{
    //    //    c.OccupiedCell.OccupiedUnit = null;
    //    //}

    //    OccupiedUnit = c;
    //    c.OccupiedCell = this;
    //}

    public void SetCellNumber(int num)
    {
        CellNumber = num;
        CellNumberText.text = num.ToString();
    }

    //public bool IsOccupied() => objectInThisGridSpace != null;

    //public void SetOccupied()
    //{
    //    IsOccupied = true;
    //}

    //public bool IsOccupiedNew() => OccupiedUnit != null;



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
        
        //if (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE && IsMove)
        //{
        //    BattleEvents.CellMoveHighlighted(this);
        //}
        //else if (BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK && IsAttack)
        //{
        //    BattleEvents.CellAttackHighlighted(this);
        //}

        if (IsMove)
        {
            BattleEvents.CellMoveHighlighted(this);
        }
        else if (IsAttack)
        {
            BattleEvents.CellAttackHighlighted(this);
        }

    }

    //private void HighlightMove()
    //{
    //    _highlight.SetActive(true);
    //    _highlightRenderer.material.color = _moveColour;
    //}

    //private void HightlightAttack()
    //{
    //    _highlight.SetActive(true);
    //    _highlightRenderer.material.color = _attackColur;
    //}

    //public void OnMouseDown()
    //{
    //    if ((IsMove && BattleSystem.State == BattleStatesEnum.PLAYER_MOVE) || (IsAttack && BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK))
    //    {
    //        BattleSystem.OnGridSelection(X, Y);
    //    }
    //}

    //public void OnMouseExit()
    //{

        

    //    if (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE && IsMove)
    //    {
    //        //   UpdateCellColour(colour);
    //        //_highlight.SetActive(false);
    //        BattleEvents.CellMoveUnhighlighted();
    //    }
    //    else if (BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK && IsAttack)
    //    {
    //        //_highlight.SetActive(false);
    //        //BattleEvents.CellMoveUnhighlighted();
    //        BattleEvents.CellAttackUnhighlighted();
    //    }
    //}


    public void OnMouseDown()
    {
        //if (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE && IsMove)
        //{
        //    BattleEvents.CellMoveSelected(this);
        //}
        //else if (BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK && IsAttack)
        //{
        //    BattleEvents.CellAttackSelected(this);
        //}

        //if (IsMove)
        //{
        //    BattleEvents.CellMoveSelected(this);
        //}
        //else if (IsAttack)
        //{
        //    BattleEvents.CellAttackSelected(this);
        //}

    }


    private void UpdateCellColour(Color newColor)
    {
        _renderer.material.color = newColor;
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
        colour = Color.green;
        UpdateCellColour(colour);
    }

    public void ResetCell()
    {
        IsMove = false;
        IsAttack = false;
        colour = initColour;
        UpdateCellColour(colour);

        //_highlight.SetActive(false);
    }


    #region Testing

    public void SetMoveText(string text)
    {
        MoveText.text = text;
    }

    #endregion

}
