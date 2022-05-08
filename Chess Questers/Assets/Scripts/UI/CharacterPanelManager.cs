using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _healthValues;

    [SerializeField] private RectTransform _healthBarFullRect;
    [SerializeField] private RectTransform _healthBarRect;

    private float _healthBarMaxWidth;

    private void OnEnable()
    {
        BattleEvents.OnTurnStart += SetupPanel;
    }

    

    private void OnDisable()
    {
        BattleEvents.OnTurnStart -= SetupPanel;
    }


    private void Awake()
    {
        _healthBarMaxWidth = _healthBarFullRect.rect.width;
    }

    private void Start()
    {
        _panel.SetActive(false);
    }


    private void SetupPanel(Creature creature)
    {
        _portrait.sprite = creature.PortraitSprite;
        _name.text = creature.Name;

        _healthValues.text = $"{creature.Health}/{creature.MaxHealth}";

        float barWidth = (float) creature.Health / creature.MaxHealth;
        barWidth = barWidth * _healthBarMaxWidth;
        _healthBarRect.sizeDelta = new Vector2(barWidth, _healthBarRect.rect.height);

        _panel.SetActive(true);


    }

}