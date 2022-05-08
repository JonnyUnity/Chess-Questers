using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleOverManager : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _panelBackground;
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private Button _continueButton;


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
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    public void ShowVictoryDetails()
    {
        _header.text = "VICTORY!";
        _continueButton.onClick.AddListener(ContinueQuest);
        AnimatePanel();
        _continueButton.interactable = true;
    }


    private void AnimatePanel()
    {
        _continueButton.interactable = false;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        LeanTween.alphaCanvas(_canvasGroup, 1f, 1f);
        LeanTween.moveY(_panelBackground.GetComponent<RectTransform>(), 0, 1f).setEaseOutQuint().setDelay(0.5f);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }


    public void ShowQuestOverDetails()
    {
        _header.text = "DEFEAT!";
        _continueButton.onClick.AddListener(EndQuest);
        AnimatePanel();
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