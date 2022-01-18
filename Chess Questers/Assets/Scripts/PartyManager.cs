using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : Singleton<PartyManager>
{

    public List<Creature> Adventurers = new();

    private bool IsAdventurerSelected;
    public Creature CurrAdventurer;

    public string[] DefaultNames = new string[] { "Streef", "Zeek", "Dot" };
    //public string[] DefaultNames = new string[] { "Streef" };

    //public void CreateAdventurers(BattleSystem battleSystem, int numAdventurers)
    //{
    //    Adventurers = new List<Adventurer>();


    //    if (numAdventurers > 0)
    //    {
    //        for (int i = 0; i < numAdventurers; i++)
    //        {
    //            string advName = "adventurer";

    //            Adventurer adv = new Adventurer();
    //            if (i < DefaultNames.Length)
    //            {
    //                advName = DefaultNames[i];
    //            }
    //            adv.moveClass = GameManager.Instance.GetRandomMoveClass();

    //           // adv.Init(advName, 100, GameManager.Instance.GetRandomMoveClass(), GameManager.Instance.GetRandomSprite(), false);

    //            Adventurers.Add(adv);
    //        }
    //    }
    //}



    public Creature GetCurrentAdventurer()
    {
        return CurrAdventurer;
    }

}
