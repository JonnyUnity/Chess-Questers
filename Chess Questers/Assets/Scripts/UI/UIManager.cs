using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Animator anim;
    [SerializeField] private GameObject BattleOverlay;
    [SerializeField] private GameObject DefeatScreen;
    [SerializeField] private GameObject VictoryScreen;  // not set yet!


    public void ShowVictoryScreen()
    {
        //BattleOverlay.SetActive(false);
        //VictoryScreen.SetActive(true);
        StartCoroutine(FadeToBlackAnimation());
    }

    public void ShowDefeatScreen()
    {
        //BattleOverlay.SetActive(false);
        //DefeatScreen.SetActive(true);
        //StartCoroutine(FadeToBlackAnimation());
    }

    private IEnumerator FadeToBlackAnimation()
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(1);
    }


}
