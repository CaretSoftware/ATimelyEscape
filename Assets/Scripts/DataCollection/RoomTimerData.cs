using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomTimerData
{
    public List<TimeInRoomX> timeInRooms = new List<TimeInRoomX>();

    public RoomTimerData(RoomTimer roomTimer) 
    { 
        timeInRooms = roomTimer.timeInRooms; 
    }
}
