using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHover : MonoBehaviour
{

    private Creature creature;

    private void Awake()
    {
        creature = GetComponent<Creature>();

        //if (TryGetComponent(out PlayerCharacter playerCharacter))
        //{
        //    creature = (Creature)playerCharacter;
        //}
        //else if (TryGetComponent(out Enemy enemy))
        //{
        //    creature = (Creature)enemy;
        //}

    }

    public void OnMouseEnter()
    {
        Debug.Log("Creature hovered!");
        BattleEvents.CreatureHovered(creature);
    }

    

    public void OnMouseExit()
    {
        Debug.Log("Creature unhovered!");
        BattleEvents.CreatureUnhovered(creature);
    }

}
