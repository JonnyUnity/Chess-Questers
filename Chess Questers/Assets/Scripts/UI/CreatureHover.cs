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
        if (creature.State == CharacterStatesEnum.PARTY_SELECT)
            return;

        BattleEvents.CreatureHovered(creature);
    }

    

    public void OnMouseExit()
    {
        if (creature.State == CharacterStatesEnum.PARTY_SELECT)
            return;

        BattleEvents.CreatureUnhovered(creature);
    }

}