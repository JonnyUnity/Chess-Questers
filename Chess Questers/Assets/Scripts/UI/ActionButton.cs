using JFlex.ChessQuesters.Core.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _toolTipAnchor;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Button _button;
    private Vector2 _anchorPosition;

    private string _actionName;
    private string _actionDescription;
    private float _timeToWait = 0.5f;

    private NewBattleAction _action;
    private WaitForSeconds _toolTipDelay;

    //private void OnEnable()
    //{
    //    BattleEvents.OnPlayerActionPerformed += ActionPerformed;
    //    BattleEvents.OnCellSelected += ActionSelected;
    //}

    //private void OnDisable()
    //{
    //    BattleEvents.OnPlayerActionPerformed -= ActionPerformed;
    //    BattleEvents.OnCellSelected -= ActionSelected;
    //}

    private void Awake()
    {
        _toolTipDelay = new WaitForSeconds(_timeToWait);
    }

    public void SetAction(NewBattleAction action)
    {
        _action = action;
        _buttonImage.sprite = _action.Icon;
        _button.interactable = _action.IsActive;

        _anchorPosition = GetComponent<RectTransform>().anchoredPosition;
        _actionName = _action.Name;
        _actionDescription = _action.Description;

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(TooltipDelay());
                
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        ActionManager.OnMouseLoseFocus();
    }


    private void ShowMessage()
    {

        Debug.Log(_action + " " + GetComponent<RectTransform>().anchoredPosition);
        var position = GetComponent<RectTransform>().anchoredPosition;

        ActionManager.OnMouseHover(_actionName, _actionDescription, gameObject.transform.position);
    }

    private IEnumerator TooltipDelay()
    {
        yield return _toolTipDelay;

        ShowMessage();


    }


    public void SelectAction()
    {

        BattleEvents.ActionSelected(_action);

    }

    public void CheckButtonCooldown()
    {
        _button.interactable = _action.IsActive;
    }

    //public void ActionSelected(GridCell cell)
    //{

    //    _button.interactable = _action.IsActive;
    //    //_action.DoAction();

    //    //BattleEvents.ActionSelected(_action);

    //}

    //public void ActionPerformed(NewBattleAction action)
    //{
    //    //if (action != _action)
    //    //    return;


    //    //_action.DoAction();
    //    _button.interactable = _action.IsActive;

    //}


}
