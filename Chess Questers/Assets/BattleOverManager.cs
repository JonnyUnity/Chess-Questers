using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleOverManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private Button _continueButton;

    private Animator _animator;

    

    private void OnEnable()
    {
        BattleEvents.OnBattleVictory += ShowVictoryDetails;
        BattleEvents.OnBattleLoss += ShowQuestOverDetails;
    }


    private void OnDisable()
    {
        BattleEvents.OnBattleVictory -= ShowVictoryDetails;
        BattleEvents.OnBattleLoss -= ShowQuestOverDetails;
    }


    void Start()
    {
        _animator = GetComponent<Animator>();    
    }




    public void ShowVictoryDetails()
    {
        _header.text = "VICTORY!";
        _continueButton.onClick.AddListener(ContinueQuest);
        _animator.SetTrigger("Show");
    }


    public void ShowQuestOverDetails()
    {
        _header.text = "DEFEAT!";
        _continueButton.onClick.AddListener(EndQuest);
        _animator.SetTrigger("Show");
    }


    public void ContinueQuest()
    {

        Debug.Log("The quest continues!");

        GameManager.Instance.EndQuest();
    }

    public void EndQuest()
    {
        // Save Quest details? High score?...

        Debug.Log("Game oveer!");
        GameManager.Instance.EndQuest();
        

    }




}
