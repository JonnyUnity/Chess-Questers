using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    [SerializeField]
    private TextMeshProUGUI AdventurerText;


    public void SetAdventurerText(string text)
    {
        AdventurerText.text = text;
    }



}
