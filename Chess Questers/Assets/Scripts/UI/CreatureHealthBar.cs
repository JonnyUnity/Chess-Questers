using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _characterCanvas;
    [SerializeField] private GameObject _characterPanel;
    [SerializeField] private InitiativeSet _initiative;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _healthValues;

    [SerializeField] private RectTransform _healthBarFullRect;
    [SerializeField] private RectTransform _healthBarRect;
    [SerializeField] private Image _healthBar;

    [SerializeField] private TextMeshProUGUI _damage;

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
        if (creature == null)
            return;


        _name.text = creature.Name;
        _currentHealth = creature.Health;
        _healthValues.text = $"{_currentHealth}/{creature.MaxHealth}";

        //float barWidth = ((float)creature.Health / (float)creature.MaxHealth) * _healthBarMaxWidth;
        float barWidth = (float)_currentHealth / creature.MaxHealth;
        barWidth *= _healthBarMaxWidth;
        //_healthBarRect.sizeDelta = new Vector2(barWidth, _healthBarRect.rect.height);
        _healthBar.fillAmount = (float)_currentHealth / creature.MaxHealth;

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
        int currentHealth = _currentHealth;

        int newHealth = Mathf.Max(0, _currentHealth - damage);

        //_healthValues.text = $"{_currentHealth}/{creature.MaxHealth}";

        _characterPanel.SetActive(true);
        float barWidth = (float)newHealth / _thisCreature.MaxHealth;
        barWidth *= _healthBarMaxWidth;

        if (_damage != null)
        {
            _damage.text = damage.ToString();

            var seq = LeanTween.sequence();
            var _colour = _damage.color;

            LeanTween.value(_damage.gameObject, 0f, 1f, 0.4f).
                setOnUpdate((float _value) => {
                    _colour.a = _value;
                    _damage.color = _colour;
                    });
            seq.append(LeanTween.moveY(_damage.GetComponent<RectTransform>(), 100, 1f).setEaseOutExpo());

            seq.insert(LeanTween.moveY(_damage.GetComponent<RectTransform>(), 20, 1f).setDelay(0.5f));
            seq.insert(
                LeanTween.value(_damage.gameObject, 1f, 0f, 1f).
                setOnUpdate((float _value) => {
                    _colour.a = _value;
                    _damage.color = _colour;
                }).setDelay(0.4f));

            //LeanTween.moveY(_damage.GetComponent<RectTransform>(), 100, 2f).setEaseOutExpo();
            //LeanTween.alphaText(_damage.GetComponent<RectTransform>(), 1, 0.4f);
        }

        _currentHealth = newHealth;

        var currentFill = _healthBar.fillAmount;
        LeanTween.value(_healthBar.gameObject, currentFill, barWidth, 1f).setOnUpdate((float f) => _healthBar.fillAmount = f).
            setEaseInOutCubic().setDelay(0.5f).setOnComplete(() => _isChangingHealth = false);
        LeanTween.value(currentHealth, newHealth, 1f).setOnUpdate((float f) => _healthValues.text = $"{(int)f}/{_thisCreature.MaxHealth}")
            .setDelay(0.5f);

    }


    private void Update()
    {
        if (!_characterPanel.activeInHierarchy)
            return;

        _characterCanvas.transform.LookAt(_cam.transform);
        _characterCanvas.transform.rotation = _cam.transform.rotation;
    }

}
