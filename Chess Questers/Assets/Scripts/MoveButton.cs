using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _toolTipAnchor;
    [SerializeField] private Button _button;
    private Vector2 _anchorPosition;

    private string _actionName;
    private string _actionDescription;
    private float _timeToWait = 0.5f;

    private MoveClass _move;
    private WaitForSeconds _toolTipDelay;


    private void Awake()
    {
        _toolTipDelay = new WaitForSeconds(_timeToWait);
    }

    public void SetAction(MoveClass move)
    {
        _move = move;
        //_buttonImage.sprite = _move.Icon;
        _button.interactable = _move.IsActive();

        _anchorPosition = GetComponent<RectTransform>().anchoredPosition;
        //_actionName = _move.Name;
        //_actionDescription = _move.Description;

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

        //Debug.Log(_move + " " + GetComponent<RectTransform>().anchoredPosition);
        var position = GetComponent<RectTransform>().anchoredPosition;

        ActionManager.OnMouseHover("Move", "move to a selected space", gameObject.transform.position);
    }

    private IEnumerator TooltipDelay()
    {
        yield return _toolTipDelay;

        ShowMessage();

    }

    public void SelectMove()
    {
        GameGrid.Instance.ClearGrid();
        BattleEvents.PlayerMoveSelected();
    }


}
