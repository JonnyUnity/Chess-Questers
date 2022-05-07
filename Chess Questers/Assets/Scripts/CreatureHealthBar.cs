using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHealthBar : MonoBehaviour
{

    [SerializeField] private GameObject _characterPanel;
    [SerializeField] private InitiativeSet _initiative;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _healthValues;

    [SerializeField] private RectTransform _healthBarFullRect;
    [SerializeField] private RectTransform _healthBarRect;

    private Camera _cam;
    private float _healthBarMaxWidth;
    private Creature _thisCreature;
    private int _currentHealth;
    private bool _isChangingHealth;


    private void OnEnable()
    {
        BattleEvents.OnCreatureHover += SetupPanel;
        BattleEvents.OnCreatureUnhover += HidePanel;
        BattleEvents.OnTakeDamageStart += ChangeHealth;
        BattleEvents.OnTakeDamageFinish += HidePanel;
    }



    private void OnDisable()
    {
        BattleEvents.OnCreatureHover -= SetupPanel;
        BattleEvents.OnCreatureUnhover -= HidePanel;
        BattleEvents.OnTakeDamageStart -= ChangeHealth;
        BattleEvents.OnTakeDamageFinish -= HidePanel;
    }


    private void Start()
    {
        _characterPanel.SetActive(false);
    }

    private void Awake()
    {
        _thisCreature = GetComponent<Creature>();
        _healthBarMaxWidth = _healthBarFullRect.rect.width;
        _cam = Camera.main;
    }


    private void SetupPanel(Creature creature)
    {
        if (creature == _initiative.ActiveCharacter)
            return;

        if (creature != _thisCreature)
            return;
       

        _name.text = creature.Name;
        _currentHealth = creature.Health;
        _healthValues.text = $"{_currentHealth}/{creature.MaxHealth}";

        //float barWidth = ((float)creature.Health / (float)creature.MaxHealth) * _healthBarMaxWidth;
        float barWidth = (float)_currentHealth / creature.MaxHealth;
        barWidth *= _healthBarMaxWidth;
        _healthBarRect.sizeDelta = new Vector2(barWidth, _healthBarRect.rect.height);

        _characterPanel.SetActive(true);

    }

    private void HidePanel(Creature creature)
    {
        if (_isChangingHealth)
            return;

        _characterPanel.SetActive(false);
    }



    public void ChangeHealth(Creature creature, int damage)
    {
        if (creature != _thisCreature)
            return;

        _isChangingHealth = true;
        _characterPanel.SetActive(true);
        _currentHealth -= damage;

        _characterPanel.SetActive(true);
        float barWidth = (float)_currentHealth / _thisCreature.MaxHealth;
        barWidth *= _healthBarMaxWidth;

        LeanTween.size(_healthBarRect, new Vector2(barWidth, _healthBarRect.rect.height), 1f).setEaseInOutCubic().setDelay(0.5f)
            .setOnComplete(() => _isChangingHealth = false);

    }


    private void Update()
    {
        if (!_characterPanel.activeInHierarchy)
            return;

        _characterPanel.transform.LookAt(_cam.transform);
        _characterPanel.transform.rotation = _cam.transform.rotation;
    }

}
