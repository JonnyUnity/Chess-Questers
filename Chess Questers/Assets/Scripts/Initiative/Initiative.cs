using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Initiative
{

    public List<Creature> Creatures { get; private set; }


    public Initiative(List<Creature> adventurers, List<Creature> enemies)
    {
        Creatures = new List<Creature>();

        foreach (Creature a in adventurers)
        {
            a.RollInitiative();
            Creatures.Add(a);
        }

        foreach (Creature e in enemies)
        {
            e.RollInitiative();
            Creatures.Add(e);
        }

        Creatures = Creatures.OrderByDescending(o => o.Initiative).ThenBy(o => o.name).ToList();

    }

    
    public Creature GetCurrentCreature()
    {
        return Creatures.First();
    }

    public Creature GetNextCreature()
    {
        Creature c = Creatures.First();
        c.ToggleSelected(false);
        Creatures.RemoveAt(0);
        Creatures.Add(c);
        Creatures.First().ToggleSelected(true);
        
        return GetCurrentCreature();
    }

    public int GetCreatureIndex(Creature c)
    {
        return Creatures.IndexOf(c);
    }

    // returns the index of the creature before removing it from the list
    // so it can also be removed from the UI list
    public int RemoveCreature(Creature c)
    {
        int index = Creatures.IndexOf(c);
        Creatures.RemoveAt(index);

        return index;
    }

    // test
    public void RemoveCreatureAtIndex(int index)
    {
        Creatures.RemoveAt(index);
    }


    public bool AllEnemiesDead()
    {
        return !Creatures.Where(w => w.IsEnemy).Any();
    }


    public bool AllAdventurersDead()
    {
        return !Creatures.Where(w => !w.IsEnemy).Any();
    }

}
