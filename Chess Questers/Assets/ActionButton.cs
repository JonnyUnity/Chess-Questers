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

    private ActionClass _action;
    private WaitForSeconds _toolTipDelay;

    private void OnEnable()
    {
        BattleEvents.OnPlayerActionPerformed += ActionPerformed;
    }

    private void OnDisable()
    {
        BattleEvents.OnPlayerActionPerformed -= ActionPerformed;
    }

    private void Awake()
    {
        _toolTipDelay = new WaitForSeconds(_timeToWait);
    }

    public void SetAction(ActionClass action)
    {
        _action = action;
        _buttonImage.sprite = _action.Icon;
        _button.interactable = _action.IsActive;

        _anchorPosition = GetComponent<RectTransform>().anchoredPosition;
        _actionName = _action.Name;
        _actionDescription = _action.Description;

     //   Debug.Log(action + " " + GetComponent<RectTransform>().anchoredPosition);

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

    public void SelectAttack()
    {
        GameGrid.Instance.ClearGrid();

        //_action.DoAction();
        //_button.interactable = _action.IsActive();
        BattleEvents.ActionSelected(_action);

    }

    public void ActionPerformed(ActionClass action)
    {
        if (action != _action)
            return;


        _action.DoAction();
        _button.interactable = _action.IsActive;

    }


}
