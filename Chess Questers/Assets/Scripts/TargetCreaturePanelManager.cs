using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetCreaturePanelManager : MonoBehaviour
{

    [SerializeField] private GameObject _targetPanel;
    [SerializeField] private InitiativeSet _initiative;

    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _healthValues;

    [SerializeField] private RectTransform _healthBarFullRect;
    [SerializeField] private RectTransform _healthBarRect;

    private float _healthBarMaxWidth;
    private Creature _previousCreature;

    private void OnEnable()
    {
        BattleEvents.OnCreatureHover += SetupPanel;
        BattleEvents.OnCreatureUnhover += HidePanel;
    }



    private void OnDisable()
    {
        BattleEvents.OnCreatureHover -= SetupPanel;
        BattleEvents.OnCreatureUnhover -= HidePanel;
    }


    private void Start()
    {
        _targetPanel.SetActive(false);
    }

    private void Awake()
    {
        _healthBarMaxWidth = _healthBarFullRect.rect.width;
    }


    private void SetupPanel(Creature creature)
    {
        if (creature == _initiative.ActiveCharacter)
            return;

        if (_previousCreature != creature)
        {
            _portrait.sprite = creature.PortraitSprite;
            _name.text = creature.Name;

            _healthValues.text = $"{creature.Health}/{creature.MaxHealth}";

            //float barWidth = ((float)creature.Health / (float)creature.MaxHealth) * _healthBarMaxWidth;
            float barWidth = (float)creature.Health / creature.MaxHealth;
            barWidth = barWidth * _healthBarMaxWidth;
            _healthBarRect.sizeDelta = new Vector2(barWidth, _healthBarRect.rect.height);

        }

        _targetPanel.SetActive(true);

    }

    private void HidePanel(Creature creature)
    {
        _targetPanel.SetActive(false);
    }

}
