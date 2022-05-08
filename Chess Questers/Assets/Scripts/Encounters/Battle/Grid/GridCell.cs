using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JFlex.ChessQuesters.Encounters.Battle.Grid
{
    public class GridCell : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector3 Position { get; private set; }

        private Color initColour = Color.white;
        private Color colour = Color.white;

        [SerializeField] private Color _highlightColour;
        [SerializeField] private Color _moveColour;
        [SerializeField] private Color _attackColur;

        private Renderer _renderer;
        public bool IsSelectable;


        public void Awake()
        {
            _renderer = GetComponent<Renderer>();
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


        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }


        public Vector3 GetPosition()
        {
            return new Vector3(X, 0.1f, Y);
        }


        private void UpdateCellColour(Color newColor)
        {
            _renderer.material.color = newColor;
        }


        public void SetAsValidMove()
        {
            IsSelectable = true;
            colour = Color.yellow;
            UpdateCellColour(colour);
        }

        public void SetAsValidAttack()
        {
            IsSelectable = true;
            colour = Color.green;
            UpdateCellColour(colour);
        }

        public void ResetCell()
        {
            IsSelectable = false;
            colour = initColour;
            UpdateCellColour(colour);
        }

    }
}