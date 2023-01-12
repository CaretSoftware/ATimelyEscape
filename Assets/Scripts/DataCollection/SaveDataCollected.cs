using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveDataCollected 
{
    private static string roomTimerFolder = Application.persistentDataPath + "/RoomTimer/";
    private static string variableFolder = Application.persistentDataPath + "/VariableFolder/";

    public static string VariableFolder { get { return variableFolder; } }
    public static int SaveRoomTimer(RoomTimer roomTimer) 
    {
        EnsureDirectoryExists(roomTimerFolder);
        int counter = 0;
        while (File.Exists(roomTimerFolder + string.Format("{0:D2}", counter) + ".data"))
        {
            counter++;
        }

        SaveRoomTimer(roomTimer, roomTimerFolder + string.Format("{0:D2}", counter) + ".data");
        return counter;
    }

    public static void SaveRoomTimer(RoomTimer roomTimer, int number)
    {
        SaveRoomTimer(roomTimer, roomTimerFolder + string.Format("{0:D2}", number) + ".data");
    }

    private static void SaveRoomTimer(RoomTimer roomTimer, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        RoomTimerData data = new RoomTimerData(roomTimer);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static RoomTimerData LoadDataCollected(int number)
    {
        string path = roomTimerFolder + string.Format("{0:D2}", number) + ".data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            RoomTimerData data = formatter.Deserialize(stream) as RoomTimerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in" + path);
            return null;
        }


    }
    private static void EnsureDirectoryExists(string filePath)
    {
        FileInfo fi = new FileInfo(filePath);
        if (!fi.Directory.Exists)
        {
            Directory.CreateDirectory(fi.DirectoryName);

        }
    }

    public static int GetNumberOfRoomSaves()
    {
        int counter = 0;
        while (File.Exists(roomTimerFolder + string.Format("{0:D2}", counter) + ".data"))
        {
            counter++;
        }
        return counter;
    }

    public static void SaveVariableData(VariablesID variables)
    {
        EnsureDirectoryExists(variableFolder);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(variableFolder + variables.name + ".data", FileMode.Create);

        formatter.Serialize(stream, variables);
        stream.Close();
    }

    public static VariablesID LoadVariableData(string fileName)
    {
        string path = variableFolder + fileName;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            VariablesID data = formatter.Deserialize(stream) as VariablesID;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in" + path);
            return null;
        }
    }

}
