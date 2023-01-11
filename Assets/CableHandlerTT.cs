using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class CableHandlerTT : MonoBehaviour
{
    [SerializeField] private GameObject past, present;

    private void Start()
    {
        if(past != null)
            past.SetActive(false);
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
    }

    private void TimeTravel(TimePeriodChanged eve)
    {
        if(past != null)
            past.SetActive((eve.from == TimeTravelPeriod.Present && eve.to == TimeTravelPeriod.Past) 
                           || (eve.from == TimeTravelPeriod.Future && eve.to == TimeTravelPeriod.Past));
        
        present.SetActive((eve.from == TimeTravelPeriod.Past && eve.to == TimeTravelPeriod.Present) 
                          || (eve.from == TimeTravelPeriod.Future && eve.to == TimeTravelPeriod.Present) 
                          || eve.to == TimeTravelPeriod.Present);
    }

    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel);
    }
}
