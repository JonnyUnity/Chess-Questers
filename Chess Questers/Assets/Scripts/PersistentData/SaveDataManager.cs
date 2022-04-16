using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager
{
    public static string directory = "/SaveData/";
    public static string filename = "ChessQuesters.json";

    public static bool Save(QuestJsonData questData)
    {

        string dir = Application.persistentDataPath + directory;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string saveData = JsonUtility.ToJson(questData);

        Debug.Log(saveData);
        File.WriteAllText(dir + filename, saveData);

        return true;
    }


    public static QuestJsonData Load()
    {
        QuestJsonData jsonData = new QuestJsonData();

        if (QuestDataExists())
        {
            string json = File.ReadAllText(Filename());
            jsonData = JsonUtility.FromJson<QuestJsonData>(json);

        }
        else
        {
            Debug.Log("Save file could not be found!");
        }

        return jsonData;
    }


    public static void Delete()
    {
        if (QuestDataExists())
        {
            File.Delete(Filename());
        }
    }


    public static string Filename()
    {
        return Application.persistentDataPath + directory + filename;
    }


    public static bool QuestDataExists()
    {
        return File.Exists(Filename());
    }

    #region Character Data conversions

    //public static List<ImprovedCharacter> DeserializeCharacterData(CharacterJsonData[] data, bool isFriendly)
    //{
    //    List<ImprovedCharacter> characters = new List<ImprovedCharacter>();
    //    foreach (CharacterJsonData c in data)
    //    {
    //        //characters.Add(new ImprovedCharacter(c));
    //    }

    //    return characters;
    //}
    

    public static CharacterJsonData[] SerializeCharacterData(List<PlayerCharacter> data)
    {
        CharacterJsonData[] characters = new CharacterJsonData[data.Count];

        for (int i = 0; i < data.Count; i++)
        {
            characters[i] = new CharacterJsonData(data[i]);
        }

        return characters;
    }

    public static NewEnemyJsonData[] SerializeEnemyData(List<Enemy> data)
    {
        NewEnemyJsonData[] enemies = new NewEnemyJsonData[data.Count];

        for (int i = 0; i < data.Count; i++)
        {
            enemies[i] = new NewEnemyJsonData(data[i]);
        }

        return enemies;

    }


    public static CharacterJsonData[] SerializeCharacterDataNew(List<Creature> data)
    {
        CharacterJsonData[] characters = new CharacterJsonData[data.Count];

        for (int i = 0; i < data.Count; i++)
        {
            characters[i] = new CharacterJsonData((PlayerCharacter)data[i]);
        }

        return characters;
    }

    public static NewEnemyJsonData[] SerializeEnemyDataNew(List<Creature> data)
    {
        NewEnemyJsonData[] enemies = new NewEnemyJsonData[data.Count];

        for (int i = 0; i < data.Count; i++)
        {
            enemies[i] = new NewEnemyJsonData((Enemy)data[i]);
        }

        return enemies;

    }

    #endregion



}
