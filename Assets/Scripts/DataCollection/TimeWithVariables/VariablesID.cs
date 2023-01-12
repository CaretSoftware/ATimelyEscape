using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class VariablesID
{
    public string name;
    public List<VariableData> data;

    public VariablesID(string name)
    {
        this.name = name;
        data= new List<VariableData>();
    }

    public void UpdateVariableData(float value, float time)
    {
        bool valueExist = false;
        foreach (VariableData v in data) 
        { 
            if(v.value == value)
            {
                valueExist = true;
                v.UpdateTime(time);
            }        
        }
        if (!valueExist) 
        {
            data.Add(new VariableData(value, time));
        }
    }

    override
    public string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine(name + ":");
        foreach (VariableData d in data)
        {
            sb.AppendLine(d.ToString());
        }
        return sb.ToString();
    }
}
