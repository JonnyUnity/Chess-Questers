using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PassButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _toolTipAnchor;

    private float _timeToWait = 0.5f;
    private WaitForSeconds _toolTipDelay;

    private void Awake()
    {
        _toolTipDelay = new WaitForSeconds(_timeToWait);
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

        var position = GetComponent<RectTransform>().anchoredPosition;

        ActionManager.OnMouseHover("Pass", "End the turn", gameObject.transform.position);
    }

    private IEnumerator TooltipDelay()
    {
        yield return _toolTipDelay;

        ShowMessage();

    }


    public void EndTurn()
    {
        GameGrid.Instance.ClearGrid();
        BattleEvents.TurnOver();
    }

    
}