using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VariableData 
{
    public float value;

    public float time;

    public VariableData(float value, float time)
    {
        this.value = value;
        this.time = time;
    }

    public void UpdateTime(float time)
    {
        this.time += time;
    }
}
