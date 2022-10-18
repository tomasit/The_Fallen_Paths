using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;

public class SerializationManager
{
    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();
        if (!Directory.Exists(Application.persistentDataPath + "/Saves")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }
        string path = Application.persistentDataPath + "/Saves/" + saveName + ".save";
        FileStream file = File.Create(path);
        formatter.Serialize(file, saveData);
        file.Close();
        return true;
    }

    public static void EraseSave(string saveName)
    {
        string fullPath = Application.persistentDataPath + "/Saves/" + saveName + ".save";
        
        if (!File.Exists(fullPath)) {
            Debug.Log("File path doesn't exist");
            return;
        }
        File.Delete(fullPath);
    }

    public static bool Exist(string path)
    {
        return File.Exists(Application.persistentDataPath + "/Saves/" + path + ".save");        
    }

    public static object Load(string path)
    {
        string fullPath = Application.persistentDataPath + "/Saves/" + path + ".save";

        if (!File.Exists(fullPath)) {
            return null;
        }
        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(fullPath, FileMode.Open);
        try {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        } catch {
            Debug.LogErrorFormat("Failed to load path at {0}", fullPath);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}