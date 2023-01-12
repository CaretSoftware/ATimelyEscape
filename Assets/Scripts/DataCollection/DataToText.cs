using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class DataToText : MonoBehaviour
{
    List<RoomTimerData> timerData;
    private string RomeTimerTxtFolder;
    [SerializeField] private int convertIndex;
    [SerializeField] private int numberOfSaves;

    [SerializeField] private string[] variablesFile;
    [SerializeField] private string variableNames;

    [ContextMenu("Number Of Room Saves")]
    private void ConvertRoomDataAtIndex()
    {
        numberOfSaves = SaveDataCollected.GetNumberOfRoomSaves();
    }

    private string ConvertRoomDataAtIndex(int convertIndex)
    {
        RoomTimerData data = SaveDataCollected.LoadDataCollected(convertIndex);
        return ConvertRoomData(data);
    }

    private string ConvertRoomData(RoomTimerData data)
    {
        if (data != null)
        {
            List<TimeInRoomX> timeInRooms = data.timeInRooms;
            StringBuilder sb = new StringBuilder();

            foreach (TimeInRoomX roomX in timeInRooms)
            {
                sb.AppendLine(roomX.ToString());
            }
            return sb.ToString();
        }
        return null;
    }

    [ContextMenu("Convert RoomData at index")]
    private void CreateRoomTxtFile()
    {
        if (ConvertRoomDataAtIndex(convertIndex) == null) return;
        CreateTxtFile(ConvertRoomDataAtIndex(convertIndex), string.Format("{0:D2}.txt", convertIndex), Application.persistentDataPath + "/RoomTimertxt/"); 
    }

    private void CreateTxtFile(string txt, string fileName, string path)
    {
        EnsureDirectoryExists(path);
        StreamWriter writer = new StreamWriter(path + fileName);
        writer.WriteLine(txt);
        writer.Close();

        Debug.Log(txt);
    }

    [ContextMenu("Convert RoomData Total")]
    private void CreateRoomDataSummery()
    {
        StringBuilder stringBuilder = new StringBuilder();
        List<TimeInRoomX> total = new();
        List<int> roomsInTotal = new();
        int counter = 0;
        RoomTimerData temp = SaveDataCollected.LoadDataCollected(counter);
        List<TimeInRoomX> tempList;

        stringBuilder.AppendLine(string.Format("Number Of Playthrough: {0:D2}", SaveDataCollected.GetNumberOfRoomSaves()));
        while (temp != null)
        {
            tempList = SaveDataCollected.LoadDataCollected(counter).timeInRooms;
            stringBuilder.AppendLine(string.Format("Playthrough {0:D2}:", counter));
            stringBuilder.AppendLine(ConvertRoomDataAtIndex(counter));

            foreach(TimeInRoomX tempX in tempList) 
            {
                bool exist = false;
                foreach (TimeInRoomX totalX in total)
                {
                    if (tempX.room == totalX.room)
                    {
                        exist = true;
                        totalX.SetTime(tempX.GetTimeInSeconds());
                        totalX.SetVisited(tempX.GetVisited());
                        break;
                    }
                }
                if (!exist)
                {
                    total.Add(new TimeInRoomX(tempX.room, tempX.GetVisited(), tempX.GetTimeInSeconds()));
                }
            }

            counter++;
            temp = SaveDataCollected.LoadDataCollected(counter);
        }
        
        stringBuilder.AppendLine("Total:");
        stringBuilder.AppendLine(ConvertRoomData(new RoomTimerData(total)));

        CreateTxtFile(stringBuilder.ToString(), "Total.txt", Application.persistentDataPath + "/RoomTimertxt/");
    }




    private void EnsureDirectoryExists(string filePath)
    {
        FileInfo fi = new(filePath);
        if (!fi.Directory.Exists)
        {
            Directory.CreateDirectory(fi.DirectoryName);
        }
    }

    [ContextMenu("Get Variable Files")]
    private void GetVeriableFileNames()
    {
        variablesFile = Directory.GetFiles(Application.persistentDataPath + "/VariableFolder/");
        for (int i = 0; i < variablesFile.Length; i++)
        {
            variablesFile[i] = Path.GetFileName(variablesFile[i]);
        }
    }

    [ContextMenu("Convert Variable Files")]
    private void ConvertVariablesToTxt()
    {
        GetVeriableFileNames();
        StringBuilder sb = new StringBuilder();
        foreach(string variable in variablesFile) 
        { 
            sb.Append(SaveDataCollected.LoadVariableData(variable).ToString());
        }
        CreateTxtFile(sb.ToString(), "Variables.txt", Application.persistentDataPath + "/Variabletxt/");
    }




}
