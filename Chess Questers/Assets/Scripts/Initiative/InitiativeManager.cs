using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitiativeManager : MonoBehaviour
{

    private GameObject TrackerObject;

    private GameObject CanvasObject;
    private GameObject PortraitPrefab;
    private TextMeshProUGUI TurnText;

    private List<GameObject> Portraits;

    private List<Creature> Creatures;

    public void Awake()
    {
        Creatures = new List<Creature>();

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

        foreach (var c in Creatures)
        {
            GameObject portraitObj = Instantiate(PortraitPrefab, CanvasObject.transform);
            Portrait portrait = portraitObj.GetComponent<Portrait>();
            portrait.SetupPortrait(c);

            Portraits.Add(portraitObj);
        }

        TurnText.text = GetCurrentCreature().CreatureName;

    }

    public Creature StartInitiative()
    {
        Creature c = GetCurrentCreature();
        c.ToggleSelected(true);

        Debug.Log(c.CreatureName + " - " + c.Initiative + " - Enemy? " + c.IsEnemy, this);
        TurnText.text = c.CreatureName;

        return c;
    }

    public void NextTurn()
    {
        Creature c = GetCurrentCreature();
        Destroy(CanvasObject.transform.GetChild(0).gameObject);

        GameObject portraitObj = Instantiate(PortraitPrefab, CanvasObject.transform);
        Portrait portrait = portraitObj.GetComponent<Portrait>();
        portrait.SetupPortrait(c);

        c = GetNextCreature();


        Debug.Log(c.name + " - " + c.Initiative + " - Enemy? " + c.IsEnemy, this);
        TurnText.text = c.name;

    }


    public bool AreAllEnemiesDead() => !Creatures.Where(w => w.IsEnemy).Any();


    public bool AreAllHeroesDead() => !Creatures.Where(w => !w.IsEnemy).Any();


    public Creature GetCurrentCreature()
    {
        return Creatures.FirstOrDefault();
    }

    public bool IsEnemyTurn()
    {
        Creature c = GetCurrentCreature();
        return c.IsEnemy;
    }

    public void RemoveCreature(Creature c)
    {
        int index = Creatures.IndexOf(c);
        Creatures.RemoveAt(index);
        if (index >= 0)
        {
            Destroy(CanvasObject.transform.GetChild(index).gameObject);
        }

    }

    public void RemoveCreatureAtIndex()
    {
        int i = CanvasObject.transform.childCount - 1;

        Creatures.RemoveAt(i);
        Destroy(CanvasObject.transform.GetChild(i).gameObject);
    }

    private Creature GetNextCreature()
    {
        Creature c = Creatures.First();
        c.ToggleSelected(false);
        Creatures.RemoveAt(0);
        Creatures.Add(c);
        Creatures.First().ToggleSelected(true);

        return GetCurrentCreature();
    }

}
