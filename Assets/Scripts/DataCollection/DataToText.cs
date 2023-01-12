using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        int totalTotalTime = 0;
        

        stringBuilder.AppendLine(string.Format("Number Of Playthrough: {0:D2}", SaveDataCollected.GetNumberOfRoomSaves()));
        while (temp != null)
        {
            tempList = SaveDataCollected.LoadDataCollected(counter).timeInRooms;
            stringBuilder.AppendLine(string.Format("Playthrough {0:D2}:\n", counter));
            stringBuilder.Append(ConvertRoomDataAtIndex(counter));
            int roomTotalTime = 0;

            foreach (TimeInRoomX tempX in tempList) 
            {
                bool exist = false;
                foreach (TimeInRoomX totalX in total)
                {
                    if (tempX.room == totalX.room && tempX.IsTutorial == totalX.IsTutorial)
                    {
                        exist = true;
                        totalX.SetTime(tempX.GetTimeInSeconds());
                        totalX.SetVisited(tempX.GetVisited());
                        
                        break;
                    }
                }
                if (!exist)
                {
                    total.Add(new TimeInRoomX(tempX.room, tempX.GetVisited(), tempX.GetTimeInSeconds(), tempX.IsTutorial));
                }
                roomTotalTime += tempX.GetTimeInSeconds();
            }

            stringBuilder.AppendLine(string.Format("Playthrough {0:D2} Total time: \n{1:D2} : {2:D2} : {3:D2} \n\n", 
                counter,
                (roomTotalTime / 3600),
                (roomTotalTime / 60) % 60,
                roomTotalTime % 60));
            counter++;
            temp = SaveDataCollected.LoadDataCollected(counter);
            totalTotalTime += roomTotalTime;
        }
        
        stringBuilder.AppendLine("Total:");
        stringBuilder.AppendLine(ConvertRoomData(new RoomTimerData(total)));

        stringBuilder.AppendLine(string.Format("Total time: \n{1:D2} : {2:D2} : {3:D2}",
                counter,
                (totalTotalTime / 3600),
                (totalTotalTime / 60) % 60,
                totalTotalTime % 60));

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
            sb.AppendLine(SaveDataCollected.LoadVariableData(variable).ToString());
        }
        CreateTxtFile(sb.ToString(), "Variables.txt", Application.persistentDataPath + "/Variabletxt/");
    }

    [ContextMenu("Convert Spesific Variable Files")]
    private void ConvertSpesificVariable()
    {
        GetVeriableFileNames();
        if (variablesFile.Contains(variableNames))
        {
            CreateTxtFile(SaveDataCollected.LoadVariableData(variableNames).ToString(), "Variables.txt", Application.persistentDataPath + "/Variabletxt/");
        }
        else
        {
            Debug.LogError(variableNames + " doesn't exist in" + Application.persistentDataPath + "/VariableFolder/");
        }
    }




}
