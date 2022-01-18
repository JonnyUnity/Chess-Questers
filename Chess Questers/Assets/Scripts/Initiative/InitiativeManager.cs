using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitiativeManager : MonoBehaviour
{

    private Initiative TurnOrder;

    private GameObject TrackerObject;

    private GameObject CanvasObject;
    private GameObject PortraitPrefab;
    private TextMeshProUGUI TurnText;

    private List<GameObject> Portraits;

    public void Awake()
    {
        PortraitPrefab = Resources.Load("Prefabs/Portrait") as GameObject;
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            CanvasObject = GameObject.Find("Portraits");
            var tmpText = GameObject.Find("CurrentCharTurn");
            TurnText = tmpText.GetComponent<TextMeshProUGUI>();
        }
    }

    public void Setup(List<Creature> adventurers, List<Creature> enemies)
    {
        Portraits = new List<GameObject>();
        TurnOrder = new Initiative(adventurers, enemies);

        foreach (var c in TurnOrder.Creatures)
        {
            GameObject portraitObj = Instantiate(PortraitPrefab, CanvasObject.transform);
            Portrait portrait = portraitObj.GetComponent<Portrait>();
            portrait.SetupPortrait(c);

            Portraits.Add(portraitObj);
        }

        TurnText.text = TurnOrder.GetCurrentCreature().CreatureName;

    }

    public Creature StartInitiative()
    {
        Creature c = TurnOrder.GetCurrentCreature();
        c.ToggleSelected(true);

        Debug.Log(c.CreatureName + " - " + c.Initiative + " - Enemy? " + c.IsEnemy);
        TurnText.text = c.CreatureName;

        return c;
    }

    public void NextTurn()
    {
        Creature c = TurnOrder.GetCurrentCreature();
        Destroy(CanvasObject.transform.GetChild(0).gameObject);

        GameObject portraitObj = Instantiate(PortraitPrefab, CanvasObject.transform);
        Portrait portrait = portraitObj.GetComponent<Portrait>();
        portrait.SetupPortrait(c);

        c = TurnOrder.GetNextCreature();

        Debug.Log(c.name + " - " + c.Initiative + " - Enemy? " + c.IsEnemy);
        TurnText.text = c.name;

    }


    public bool AllEnemiesDead()
    {
        return TurnOrder.AllEnemiesDead();
    }

    public bool AllAdventurersDead()
    {
        return TurnOrder.AllAdventurersDead();
    }

    public Creature GetCurrentCreature()
    {
        return TurnOrder.GetCurrentCreature();
    }

    public bool IsEnemyTurn()
    {
        Creature c = TurnOrder.GetCurrentCreature();
        return c.IsEnemy;
    }

    public void RemoveCreature(Creature c)
    {
        int index = TurnOrder.RemoveCreature(c);
        if (index >= 0)
        {
            Destroy(CanvasObject.transform.GetChild(index).gameObject);
        }
        
    }

    public void RemoveCreatureAtIndex()
    {
        int i = CanvasObject.transform.childCount - 1;

        TurnOrder.RemoveCreatureAtIndex(i);
        Destroy(CanvasObject.transform.GetChild(i).gameObject);
    }

}
