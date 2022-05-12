using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using JFlex.ChessQuesters.Core.ScriptableObjects;
using JFlex.ChessQuesters.Encounters.Battle.Grid;

public class BattleActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _toolTipAnchor;
    [SerializeField] private GameObject _toolTipWindow;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _contentText;

    [SerializeField] private Image _buttonImage;

    private float _timeToWait = 0.5f;

    private NewBattleAction _action;
    private WaitForSeconds _toolTipDelay;

    private LayoutElement _layoutElement;
    private RectTransform _buttonRectTransform;
    private RectTransform _toolTipRectTransform;

    private int _characterSlot;
    private string _name;
    private string _description;


    private void Awake()
    {
        _toolTipDelay = new WaitForSeconds(_timeToWait);
        _layoutElement = _toolTipWindow.GetComponent<LayoutElement>();
        _buttonRectTransform = GetComponent<RectTransform>();
        _toolTipRectTransform = _toolTipWindow.GetComponent<RectTransform>();
    }

    private void Start()
    {
        HideToolTip();
    }


    public void SetAction(NewBattleAction action, int characterSlot)
    {
        _action = action;
        _buttonImage.sprite = _action.Icon;
        _headerText.text = _action.Name;
        _contentText.text = _action.Description;

        _characterSlot = characterSlot;
        _name = _action.Name;
        _description = _action.Description;

        _layoutElement.enabled = (_headerText.text.Length > 80 || _contentText.text.Length > 80);
        _toolTipRectTransform.ForceUpdateRectTransforms();

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(TooltipDelay());
                
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        HideToolTip();
    }


    private void ShowMessage()
    {
        _toolTipWindow.gameObject.SetActive(true);

        Vector3 pos = gameObject.transform.position;
        Vector3 worldPos = _buttonRectTransform.TransformPoint(pos);

        Vector3 toolTipPosition = pos;
        
        if (toolTipPosition.x + _toolTipRectTransform.rect.width > Screen.width)
        {
            toolTipPosition.x = Screen.width - _toolTipRectTransform.rect.width;
        }

        _toolTipWindow.transform.position = toolTipPosition;
    }

    private IEnumerator TooltipDelay()
    {
        yield return _toolTipDelay;

        //ShowMessage();
        ActionDisplay.OnMouseHover(_name, _description, _characterSlot);


    }


    public void SelectAttack()
    {
        GameGrid.Instance.ClearGrid();
    }

    private void HideToolTip()
    {
        ActionDisplay.OnMouseLoseFocus();
        //_toolTipWindow.gameObject.SetActive(false);
    }


}