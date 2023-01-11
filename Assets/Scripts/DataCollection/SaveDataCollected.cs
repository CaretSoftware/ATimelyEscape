using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public static class SaveDataCollected 
{
    private static string RomeTimerFolder = Application.persistentDataPath + "/RoomTimer/";
    public static int SaveRoomTimer(RoomTimer roomTimer) 
    {
        EnsureDirectoryExists(RomeTimerFolder);
        int counter = 0;
        while (File.Exists(RomeTimerFolder + string.Format("{0:D2}", counter) + ".data"))
        {
            counter++;
        }

        SaveRoomTimer(roomTimer, RomeTimerFolder + string.Format("{0:D2}", counter) + ".data");
        return counter;
    }

    public static void SaveRoomTimer(RoomTimer roomTimer, int number)
    {
        SaveRoomTimer(roomTimer, RomeTimerFolder + string.Format("{0:D2}", number) + ".data");
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
        string path = RomeTimerFolder + string.Format("{0:D2}", number) + ".data";
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

    public static int GetNumberOfSaves()
    {
        int counter = 0;
        while (File.Exists(RomeTimerFolder + string.Format("{0:D2}", counter) + ".data"))
        {
            counter++;
        }
        return counter;
    }
}
