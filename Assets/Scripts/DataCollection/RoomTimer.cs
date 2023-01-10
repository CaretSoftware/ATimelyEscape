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


    private void Start()
    {
        timeInRooms = new List<TimeInRoomX>();
        currentRoomIndex = runtimeSceneManager.GetCurrentSceneIndex();
        currentRoom = new TimeInRoomX(currentRoomIndex);
        timeInRooms.Add(currentRoom);
        saveNumber = SaveDataCollected.SaveRoomTimer(this);
        StartCoroutine(Save(saveEvery));
    }

    private void Update()
    {
        if (currentRoomIndex != runtimeSceneManager.GetCurrentSceneIndex())
        {
            currentRoomIndex = runtimeSceneManager.GetCurrentSceneIndex();

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
            
        }
        currentRoom.SetTime(Time.deltaTime);
    }

    private IEnumerator Save(int timer)
    {
        yield return new WaitForSeconds(timer);
        SaveDataCollected.SaveRoomTimer(this, saveNumber);
        StartCoroutine(Save(timer));
    }

    [ContextMenu("Load")]
    private void Load()
    {
        RoomTimerData roomTimerData = SaveDataCollected.LoadDataCollected(saveNumber);
        if (roomTimerData != null) { timeInRooms = roomTimerData.timeInRooms; }
        else timeInRooms = new List<TimeInRoomX>();
    }


    
}
