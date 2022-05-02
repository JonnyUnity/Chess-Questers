using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverManager : MonoBehaviour
{
    [SerializeField] private RectTransform _toolTipWindow;
    [SerializeField] private TextMeshProUGUI _toolTip;

    public static Action<string, Vector2> OnMouseHover;
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


    // Start is called before the first frame update
    void Start()
    {
        HideToolTip();
    }

    private void ShowToolTip(string toolTip, Vector2 position)
    {
        _toolTip.text = toolTip;
        var width = _toolTip.preferredWidth > 200 ? 200 : _toolTip.preferredWidth;
        _toolTipWindow.sizeDelta = new Vector2(width, _toolTip.preferredHeight);

        _toolTipWindow.gameObject.SetActive(true);
        _toolTipWindow.transform.position = new Vector2(position.x + _toolTipWindow.sizeDelta.x * 2, position.y);

    }


    private void HideToolTip()
    {
        _toolTip.text = default;
        _toolTipWindow.gameObject.SetActive(false);
    }
}
