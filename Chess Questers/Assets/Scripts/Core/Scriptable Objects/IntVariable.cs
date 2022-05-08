using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Chess Questers/Int Variable")]
    public class IntVariable : ScriptableObject
    {
        private int InitialValue;
        public int Value; 
        
        private void OnEnable()
        {
            Value = InitialValue;
        }
        public void SetValue(int value)
        {
            Value = value;
        }
        public void Inc()
        {
            Value++;
        }
        public void Dec()
        {
            Value--;
        }
    }
}
