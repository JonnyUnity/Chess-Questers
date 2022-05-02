using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreaturePortrait : MonoBehaviour
{

    [SerializeField] private Color _friendlyColour;
    [SerializeField] private Color _enemyColour;
    [SerializeField] private Image _background;
    [SerializeField] private Image _portrait;

    public int ID { get; private set; }

    public void SetPortrait(Creature creature)
    {
        ID = creature.ID;
        _background.color = creature.IsFriendly ? _friendlyColour : _enemyColour;
        _portrait.sprite = creature.PortraitSprite;
    }

}
