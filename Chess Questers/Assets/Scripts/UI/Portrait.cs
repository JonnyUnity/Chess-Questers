using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Portrait : MonoBehaviour
{

    [SerializeField] private Image BorderImage;
    [SerializeField] private Image BackgroundColour;
    [SerializeField] private Image CharacterImage;

    private Creature Creature;

    public void SetupPortrait(Creature creature)
    {
        Creature = creature;

        //if (creature.IsEnemy)
        //{
        //    BorderImage.color = Color.red;
        //    BackgroundColour.color = Color.red;
        //} 
        //else
        //{
        //    BorderImage.color = Color.blue;
        //    BackgroundColour.color = Color.blue;
        //}

        //CharacterImage.sprite = creature.PortraitSprite;
    }


}
