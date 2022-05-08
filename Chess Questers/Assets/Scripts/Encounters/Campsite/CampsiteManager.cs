using JFlex.ChessQuesters.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Encounters.Campsite
{
    public class CampsiteManager : MonoBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {
            // save data on scene load

        }

        public void LoadDestination(int encounterType)
        {
            GameManager.Instance.GoToEncounterType((EncounterTypesEnum)encounterType);
        }

        public void QuitToTitle()
        {
            GameManager.Instance.QuitToTitle();
        }

    }
}