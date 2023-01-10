using System;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomTimer : MonoBehaviour
{
    private int saveNumber;
    [SerializeField] private RuntimeSceneManager runtimeSceneManager;

    [SerializeField] public List<TimeInRoomX> timeInRooms = new List<TimeInRoomX>();
    private TimeInRoomX currentRoom;

    private int currentRoomIndex;

    [SerializeField] private int saveEvery = 10;


    private void Start()
    {
        currentRoomIndex = runtimeSceneManager.GetCurrentSceneIndex();
        currentRoom = new TimeInRoomX(currentRoomIndex);
        timeInRooms.Add(currentRoom);
        saveNumber = SaveDataCollected.SaveRoomTimer(this);
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
        currentRoom.SetTime();
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
        timeInRooms = SaveDataCollected.LoadDataCollected(saveNumber).timeInRooms;
    }


    [Serializable]
    public class TimeInRoomX
    {
        public int room;

        [SerializeField] private int visited;

        private float totalTime;

        [SerializeField] private int hours;
        [SerializeField] private int minutes;
        [SerializeField] private int seconds;
        [SerializeField] private int milliseconds;

        public TimeInRoomX(int x) { this.room = x; visited++; }

        public void SetTime()
        {
            totalTime += Time.deltaTime;
            hours = (int)(totalTime / 3600); minutes = (int)(totalTime / 60); seconds = (int)totalTime; milliseconds = ((int)(totalTime * 1000)) % 1000;
        }

        public void Visited() { visited++; }
    }
}
