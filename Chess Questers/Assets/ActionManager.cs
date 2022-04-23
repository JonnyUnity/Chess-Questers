using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionsContainer;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private RectTransform _toolTipWindow;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Vector3 _toolTipOffset;

    [SerializeField] private InitiativeSet _initiative;

    private LayoutElement _layoutElement;

    private List<GameObject> _actionButtons;
    private List<ActionClass> _actions;

    public static Action<string, string, Vector3> OnMouseHover;
    public static Action OnMouseLoseFocus;
    
    private void OnEnable()
    {
        OnMouseHover += ShowToolTip;
        OnMouseLoseFocus += HideToolTip;
        BattleEvents.OnPlayerStartTurn += SetActions;
    }

    private void OnDisable()
    {
        OnMouseHover -= ShowToolTip;
        OnMouseLoseFocus -= HideToolTip;
        BattleEvents.OnPlayerStartTurn -= SetActions;
    }

    private void Awake()
    {
        _layoutElement = _toolTipWindow.GetComponent<LayoutElement>();
    }

    private void Start()
    {
        _actionButtons = new List<GameObject>();
        HideToolTip();
    }

    public void SetActions()
    {
        GameGrid.Instance.ShowGrid();
        _actions = _initiative.ActiveCharacter.Actions;

        foreach (GameObject actionButton in _actionButtons)
        {
            Destroy(actionButton);
        }

        foreach (ActionClass action in _actions)
        {
            Debug.Log(action);
            var buttonObj = Instantiate(_buttonPrefab, _actionsContainer.transform);
            buttonObj.GetComponent<ActionButton>().SetAction(action);
            _actionButtons.Add(buttonObj);
        }

    }


    private void ShowToolTip(string header, string content, Vector3 position)
    {
        _headerText.text = header;
        _contentText.text = content;

        _layoutElement.enabled = (header.Length > 80 || content.Length > 80);

        //var width = _toolTip.preferredWidth > 300 ? 300 : _toolTip.preferredWidth;
        //_toolTipWindow.sizeDelta = new Vector2(width, _toolTip.preferredHeight);
        Debug.Log($"{_toolTipWindow.sizeDelta}");

        //_layoutElement.enabled = (toolTip.Length > 80);

        _toolTipWindow.transform.position = position + _toolTipOffset;
        _toolTipWindow.gameObject.SetActive(true);
        
    }


    private void HideToolTip()
    {
        _headerText.text = default;
        _contentText.text = default;
        _toolTipWindow.gameObject.SetActive(false);
    }

}