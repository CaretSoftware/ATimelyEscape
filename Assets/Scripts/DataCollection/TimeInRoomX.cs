using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void SetTime(float x)
    {
        totalTime += x;
        hours = (int)(totalTime / 3600); minutes = (int)(totalTime / 60); seconds = (int)totalTime; milliseconds = ((int)(totalTime * 1000)) % 1000;
    }

    public void Visited() { visited++; }

}
