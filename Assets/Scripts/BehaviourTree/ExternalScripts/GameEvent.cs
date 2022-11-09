using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent
{
    public string EventDescription;
}

public abstract class ObjectiveEvent : GameEvent
{
    public string ObjectiveName;

    public ObjectiveEvent(string ObjectiveName) 
    {
        this.ObjectiveName = ObjectiveName;
    }
}


