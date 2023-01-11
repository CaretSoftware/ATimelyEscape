using System;
using System.Collections;
using System.Collections.Generic;
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
}
