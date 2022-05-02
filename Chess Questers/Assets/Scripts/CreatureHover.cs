using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHover : MonoBehaviour
{

    private Creature creature;

    private void Awake()
    {
        creature = GetComponent<Creature>();
    }

    public void OnMouseEnter()
    {
        BattleEvents.CreatureHovered(creature);
    }

    

    public void OnMouseExit()
    {
        BattleEvents.CreatureUnhovered(creature);
    }

}