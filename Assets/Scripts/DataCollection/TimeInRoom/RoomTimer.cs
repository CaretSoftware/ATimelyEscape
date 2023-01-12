using System;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomTimer : MonoBehaviour
{
    [SerializeField] private int saveNumber;
    [SerializeField] private RuntimeSceneManager runtimeSceneManager;

    [SerializeField] public List<TimeInRoomX> timeInRooms = new List<TimeInRoomX>();
    private TimeInRoomX currentRoom;

    private int currentRoomIndex;

    [SerializeField] private int saveEvery = 10;

    private float timer;


    private void Start()
    {
        timer = 0;
        timeInRooms = new List<TimeInRoomX>();
        currentRoomIndex = runtimeSceneManager.CurrentSceneIndex;
        currentRoom = new TimeInRoomX(currentRoomIndex);
        timeInRooms.Add(currentRoom);
        saveNumber = SaveDataCollected.SaveRoomTimer(this);   
    }

    private void Update()
    {
        if (currentRoomIndex != runtimeSceneManager.CurrentSceneIndex)
        {
            currentRoom.SetTime(timer);
            timer = 0; 
            currentRoomIndex = runtimeSceneManager.CurrentSceneIndex;

            bool alreadyExist = false;
            foreach (TimeInRoomX x in timeInRooms)
            {
                if (x.room == currentRoomIndex)
                {
                    alreadyExist = true;
                    currentRoom = x;
                    currentRoom.Visited();
                    break;
                }
            }
            if (!alreadyExist)
            {
                currentRoom = new TimeInRoomX(currentRoomIndex);
                timeInRooms.Add(currentRoom);
            }
            SaveDataCollected.SaveRoomTimer(this, saveNumber);
        }
        timer += Time.deltaTime;
    }

   
    private void OnDestroy()
    {
        currentRoom.SetTime(timer);
        SaveDataCollected.SaveRoomTimer(this, saveNumber);
    }

    [ContextMenu("Load")]
    private void Load()
    {
        RoomTimerData roomTimerData = SaveDataCollected.LoadDataCollected(saveNumber);
        if (roomTimerData != null) { timeInRooms = roomTimerData.timeInRooms; }
        else timeInRooms = new List<TimeInRoomX>();
    }


    
}
