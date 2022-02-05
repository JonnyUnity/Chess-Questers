using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject BattleOverlay;
    [SerializeField] private GameObject DefeatScreen;
    [SerializeField] private GameObject VictoryScreen;  // not set yet!



    public void ShowDefeatScreen()
    {

        BattleOverlay.SetActive(false);
        DefeatScreen.SetActive(true);

    }

}
