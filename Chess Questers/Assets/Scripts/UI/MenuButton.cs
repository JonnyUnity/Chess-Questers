using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private TextMeshProUGUI _text;
    private Color _orginalColour;
    [SerializeField] private Color _highlightColor;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _orginalColour = _text.color;
    }

        
    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _orginalColour;
    }

}
