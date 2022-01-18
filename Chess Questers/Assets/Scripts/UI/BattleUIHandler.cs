using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI StateText;
    [SerializeField] private TextMeshProUGUI CharacterText;


    [SerializeField] private GameObject AttackList;
    [SerializeField] private GameObject Attack1Button;
    [SerializeField] private GameObject Attack2Button;
    [SerializeField] private GameObject Attack3Button;
    [SerializeField] private GameObject Attack4Button;


    public void UpdateStateText(string text)
    {
        StateText.text = text;
    }

    public void UpdateCharacterText(string text)
    {
        CharacterText.text = text;
    }

    public void BuildAttackList(Creature adv)
    {
        // Update Attack buttons based on adventurer's attacks...

        Attack1Button.SetActive(true);

        var btn = Attack1Button.GetComponent<Button>();
        var btnText = Attack1Button.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = "ATTACK!";

        Attack2Button.SetActive(false);
        Attack3Button.SetActive(false);
        Attack4Button.SetActive(false);

        AttackList.SetActive(true);

    }

}
