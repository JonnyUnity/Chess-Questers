using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using JFlex.ChessQuesters.Core.ScriptableObjects;

public class ActionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _actionsContainer;
    [SerializeField] private GameObject _buttonPrefab;

    [SerializeField] private RectTransform _toolTipWindow;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Vector3 _toolTipOffset;

    private LayoutElement _layoutElement;
    private RectTransform _backgroundRectTransform;
    private RectTransform _canvasRectTransform;

    private List<GameObject> _actionButtons;

    public static Action<string, string, Vector3> OnMouseHover;
    public static Action OnMouseLoseFocus;
    
    private void OnEnable()
    {
        OnMouseHover += ShowToolTip;
        OnMouseLoseFocus += HideToolTip;
    }

    private void OnDisable()
    {
        OnMouseHover -= ShowToolTip;
        OnMouseLoseFocus -= HideToolTip;
    }

    private void Awake()
    {
        _layoutElement = _toolTipWindow.GetComponent<LayoutElement>();
        _backgroundRectTransform = _toolTipWindow.GetComponent<RectTransform>();
        _canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();

    }

    private void Start()
    {
        _actionButtons = new List<GameObject>();
        HideToolTip();
    }



    public void SetActions(PlayerClass playerClass)
    {

        foreach (GameObject actionButton in _actionButtons)
        {
            Destroy(actionButton);
        }



        // add the move button.
        var moveButtonObj = Instantiate(_buttonPrefab, _actionsContainer.transform);
        moveButtonObj.GetComponent<BattleActionButton>().SetAction(playerClass.MoveAction);
        _actionButtons.Add(moveButtonObj);

        foreach (NewBattleAction action in playerClass.AvailableActions)
        {
            Debug.Log(action);
            var buttonObj = Instantiate(_buttonPrefab, _actionsContainer.transform);
            buttonObj.GetComponent<BattleActionButton>().SetAction(action);
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
        //Debug.Log($"{_toolTipWindow.sizeDelta}");

        //_layoutElement.enabled = (toolTip.Length > 80);

        if (position.x + _backgroundRectTransform.rect.width > _canvasRectTransform.rect.width)
        {
            position.x = _canvasRectTransform.rect.width - _backgroundRectTransform.rect.width;
        }

        _toolTipWindow.transform.position = position + _toolTipOffset;
        //_toolTipWindow.anchoredPosition = position + _toolTipOffset;

        _toolTipWindow.gameObject.SetActive(true);
        
    }


    private void HideToolTip()
    {
        _headerText.text = default;
        _contentText.text = default;
        _toolTipWindow.gameObject.SetActive(false);
    }

}