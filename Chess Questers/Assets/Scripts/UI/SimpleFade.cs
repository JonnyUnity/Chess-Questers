using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFade : MonoBehaviour
{

    [SerializeField] private CanvasGroup _canvasGroup;

    void Start()
    {
        FadeIn(null);
    }

    private void OnEnable()
    {
        BattleEvents.OnFadeIn += FadeIn;
        BattleEvents.OnFadeOut += FadeOut;
    }


    private void OnDisable()
    {
        BattleEvents.OnFadeIn -= FadeIn;
        BattleEvents.OnFadeOut -= FadeOut;
    }

    public void FadeIn(Action action)
    {
        LeanTween.alphaCanvas(_canvasGroup, 0, 1)
            .setOnComplete(action);
    }

    public void FadeOut(Action action)
    {
       LeanTween.alphaCanvas(_canvasGroup, 1, 1)
            .setOnComplete(action);
    }

}
