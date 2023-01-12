using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class TimeInRoomX
{
    
    public int room;

    private bool isTutorial = false;
    public bool IsTutorial { get { return isTutorial; } }

    [SerializeField] private int visited;

    private float totalTime;

    [SerializeField] private int hours;
    [SerializeField] private int minutes;
    [SerializeField] private int seconds;

    public TimeInRoomX(int x) { this.room = x; visited++; }
    public TimeInRoomX(int x, int visited) { this.room = x; }

    public TimeInRoomX(int x, int visited, float totalTime) { this.room = x; this.visited = visited; SetTime(totalTime); }

    public TimeInRoomX(int x, bool isTutorial) { this.room = x; visited++; this.isTutorial = isTutorial; }
    public TimeInRoomX(int x, int visited, bool isTutorial) { this.room = x; this.isTutorial = isTutorial; }

    public TimeInRoomX(int x, int visited, float totalTime, bool isTutorial) { this.room = x; this.visited = visited; SetTime(totalTime); this.isTutorial = isTutorial; }

    public void SetTime(float x)
    {
        totalTime += x;
        hours = (int)(totalTime / 3600); minutes = (int)(totalTime / 60) % 60; seconds = (int)totalTime % 60;
    }

    public int GetTimeInSeconds()
    {
        return (int)totalTime;
    }

    public void Visited() { visited++; }

    public void SetVisited(int visited) { this.visited += visited; }
    public int GetVisited() { return visited; }

    override
    public string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Format("Room {0:D2}:", room));
        stringBuilder.AppendLine(string.Format("Visited: {0:D2}", visited));
        stringBuilder.AppendLine(string.Format("Time in Room: {0:D2} : {1:D2} : {2:D2} ", hours, minutes, seconds));

        return stringBuilder.ToString();
    }

    public string ToString(string name)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(name);
        stringBuilder.AppendLine(string.Format("Visited: {0:D2}", visited));
        stringBuilder.AppendLine(string.Format("Time in Room: {0:D2} : {1:D2} : {2:D2} ", hours, minutes, seconds));

        return stringBuilder.ToString();
    }



}
