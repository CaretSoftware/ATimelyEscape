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
        int counter = 0;
        while (File.Exists(RomeTimerFolder + string.Format("{0:D2}", counter) + ".data"))
        {
            counter++;
        }

        SaveRoomTimer(roomTimer, counter);
        return counter;
    }

    public static void SaveRoomTimer(RoomTimer roomTimer, int number)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = RomeTimerFolder + string.Format("{0:D2}", number) + ".data";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, roomTimer);
        stream.Close();
    }

    public static RoomTimer LoadDataCollected(int number)
    {
        string path = RomeTimerFolder + string.Format("{0:D2}", number) + ".data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            RoomTimer roomTimer = formatter.Deserialize(stream) as RoomTimer;
            stream.Close();

            return roomTimer;
        }
        else
        {
            Debug.LogError("Save file not found in" + path);
            return null;
        }
    }
}
