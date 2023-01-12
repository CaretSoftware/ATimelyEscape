using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    override
    public string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(value.ToString());
        sb.Append("; ");
        sb.AppendLine(String.Format("{0:D2} : {1:D2} : {2:D2}",
            (int)(time / 3600),
            (int)(time / 60) % 60,
            (int)time % 60));
        return sb.ToString();
    }
}
