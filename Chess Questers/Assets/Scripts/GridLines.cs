using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLines : MonoBehaviour
{

    private void OnEnable()
    {
        BattleEvents.OnPlayerStartTurn += ShowGridLines;
        BattleEvents.OnPlayerEndTurn += HideGridLines;
    }

    private void OnDisable()
    {
        BattleEvents.OnPlayerStartTurn -= ShowGridLines;
        BattleEvents.OnPlayerEndTurn -= HideGridLines;
    }

    private void ShowGridLines()
    {
        gameObject.SetActive(true);
    }

    private void HideGridLines()
    {
        gameObject.SetActive(false);
    }

}
