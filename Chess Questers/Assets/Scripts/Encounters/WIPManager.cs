using JFlex.ChessQuesters.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Encounters
{
    public class WIPManager : MonoBehaviour
    {
        
        public void BackToTitle()
        {
            SaveDataManager.Delete();
            GameManager.Instance.QuitToTitle();
        }

    }
}