using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

public class GameSaveManager
{

    public SudokuInfo info;

    public void SaveGame(SudokuMain main)
    {
        string path = Application.persistentDataPath;
        Debug.Log(path);
        if (!Directory.Exists(path + "/Save"))
        {
            Directory.CreateDirectory(path + "/Save");
        }

        info = main.StoreInfo();

        //二进制序列化
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Create(path + "/Save" + "/" + StaticValue.Get()._Filename);

        var jsonData = JsonUtility.ToJson(info);

        formatter.Serialize(file, jsonData);

        file.Close();

    }

    public void LoadGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        info = new SudokuInfo();

        string path = Application.persistentDataPath + "/Save" + "/" + StaticValue.Get()._Filename;
        if (File.Exists(path))
        {
            FileStream file = File.Open(path, FileMode.Open);

            JsonUtility.FromJsonOverwrite((string)formatter.Deserialize(file), info);

            file.Close();
        }
        else
        {
            Debug.Log("No such file!");
        }
    }
}