using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager
{
    public static string directory = "/SaveData/";
    public static string filename = "ChessQuesters.json";

    //public static bool Save(QuestData questData)
    //{

    //    string dir = Application.persistentDataPath + directory;
    //    if (!Directory.Exists(dir))
    //    {
    //        Directory.CreateDirectory(dir);
    //    }

    //    QuestJsonData jsonData = new QuestJsonData(questData);

    //    string saveData = JsonUtility.ToJson(jsonData);

    //    Debug.Log(saveData);
    //    File.WriteAllText(dir + filename, saveData);

    //    return true;
    //}

    public static bool SaveNew(QuestJsonData questData)
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


    //public static QuestData Load()
    //{
    //    QuestData questData = new QuestData();
    //    QuestJsonData jsonData = new QuestJsonData();

    //    if (QuestDataExists())
    //    {
    //        string json = File.ReadAllText(Filename());
    //        jsonData = JsonUtility.FromJson<QuestJsonData>(json);

    //        questData = new QuestData(jsonData);

    //    }
    //    else
    //    {
    //        Debug.Log("Save file could not be found!");
    //    }

    //    return questData;
    //}

    public static QuestJsonData LoadNew()
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

    public static List<ImprovedCharacter> DeserializeCharacterData(CharacterJsonData[] data, bool isFriendly)
    {
        List<ImprovedCharacter> characters = new List<ImprovedCharacter>();
        foreach (CharacterJsonData c in data)
        {
            //characters.Add(new ImprovedCharacter(c));
        }

        return characters;
    }
    

    public static CharacterJsonData[] SerializeCharacterData(List<ImprovedCharacter> data)
    {
        CharacterJsonData[] characters = new CharacterJsonData[data.Count];

        for (int i = 0; i < data.Count; i++)
        {
            characters[i] = new CharacterJsonData(data[i]);
        }

        return characters;
    }

    public static CharacterJsonData[] SerializeEnemyData(Enemy[] data)
    {
        CharacterJsonData[] characters = new CharacterJsonData[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            characters[i] = new CharacterJsonData(data[i]);
        }

        return characters;

    }

    #endregion

    

}
