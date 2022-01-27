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

    [SerializeField] GameObject _highlight;

    [SerializeField] private Color _highlightColour;
    [SerializeField] private Color _moveColour;
    [SerializeField] private Color _attackColur;

    // Saves a reference to the gameobject that gets placed on this cell.
    public Creature OccupiedUnit = null;

    //public bool IsOccupied;
    public bool IsMove;
    public bool IsAttack;

    private Renderer _renderer;
    private Renderer _highlightRenderer;
    private Material _highlightMaterial;

    private BattleSystem BattleSystem;

    [SerializeField]
    private TextMeshProUGUI CellNumberText;
    [SerializeField]
    private TextMeshProUGUI MoveText;

    public void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _highlightRenderer = _highlight.GetComponent<Renderer>();
        _highlightMaterial = _highlight.GetComponent<Renderer>().GetComponent<Material>();
    }

    public void Setup(BattleSystem battleSystem, int x, int y, Color color)
    {
        BattleSystem = battleSystem;
        X = x;
        Y = y;

        InitColour(color);

    }

    public void InitColour(Color newColour)
    {
        initColour = newColour;
        colour = initColour;
        UpdateCellColour(colour);
    }


    public void SetUnit(Creature c)
    {
        if (c.OccupiedCell != null)
        {
            c.OccupiedCell.OccupiedUnit = null;
        }

        OccupiedUnit = c;
        c.OccupiedCell = this;
    }

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

    public bool IsOccupiedNew() => OccupiedUnit != null;



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
            //UpdateCellColour(_highlightColour);
            //_highlight.SetActive(true);
            HighlightMove();
            BattleSystem.CharacterLook(X, Y);
        }
        else if (BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK && IsAttack)
        {
            HightlightAttack();
            BattleSystem.CharacterLook(X, Y);
        }

    }

    private void HighlightMove()
    {
        _highlight.SetActive(true);
        //_highlightMaterial.color = _moveColour;
        _highlightRenderer.material.color = _moveColour;
    }

    private void HightlightAttack()
    {
        _highlight.SetActive(true);
        //_highlightMaterial.color = _attackColur;
        _highlightRenderer.material.color = _attackColur;
    }

    public void OnMouseDown()
    {
        if ((IsMove && BattleSystem.State == BattleStatesEnum.PLAYER_MOVE) || (IsAttack && BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK))
        {
            BattleSystem.OnGridSelection(X, Y);
        }
    }

    public void OnMouseExit()
    {

        

        if (BattleSystem.State == BattleStatesEnum.PLAYER_MOVE && IsMove)
        {
            //   UpdateCellColour(colour);
            _highlight.SetActive(false);
        }
        else if (BattleSystem.State == BattleStatesEnum.PLAYER_ATTACK && IsAttack)
        {
            _highlight.SetActive(false);
        }
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
        colour = Color.red;
        UpdateCellColour(colour);
    }

    public void ResetCell()
    {
        IsMove = false;
        IsAttack = false;
        colour = initColour;
        UpdateCellColour(colour);
        _highlight.SetActive(false);
    }


    #region Testing

    public void SetMoveText(string text)
    {
        MoveText.text = text;
    }

    #endregion

}
