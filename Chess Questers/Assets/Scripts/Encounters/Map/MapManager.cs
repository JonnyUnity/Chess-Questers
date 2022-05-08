using JFlex.ChessQuesters.Core;
using JFlex.ChessQuesters.Core.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JFlex.ChessQuesters.Encounters.Map
{
    public class MapManager : MonoBehaviour
    {

        private QuestJsonData _questData;

        private void Start()
        {
            _questData = SaveDataManager.Load();    
        }

        public void LoadEncounter(Encounter encounter)
        {
            //Encounter encounter = GameManager.Instance.GetEncounter(encounterID);
            //_questData.SetNextEncounter(encounter);

            //GameManager.Instance.GoToNextEncounter();
            GameManager.Instance.GoToEncounter(encounter);

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