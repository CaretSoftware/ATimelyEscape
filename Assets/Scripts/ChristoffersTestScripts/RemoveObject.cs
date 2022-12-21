using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class RemoveObject : MonoBehaviour
{
    private void Start()
    {
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
    }

    private void TimeTravel(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Past)
        {
            Destroy(gameObject);  
        }
    }

    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel);
    }
}
