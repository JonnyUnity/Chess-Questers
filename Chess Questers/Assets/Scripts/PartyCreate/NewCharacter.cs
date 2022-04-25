using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacter
{

    public string Name { get; private set; }
    public PlayerClass PlayerClass { get; private set; }
    public int CreatureModelID { get; private set; }


    public NewCharacter(string name, PlayerClass playerClass, int creatureModelID)
    {
        Name = name;
        PlayerClass = playerClass;
        CreatureModelID = creatureModelID;
    }


    public CharacterJsonData GetJson()
    {
        CharacterJsonData jsonData = new CharacterJsonData(Name, CreatureModelID, PlayerClass);

        return jsonData;

    }

}
