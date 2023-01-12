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
        if(past != null && present != null)
            past.SetActive(false);
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
    }

    private void TimeTravel(TimePeriodChanged eve)
    {
        if(past != null)
            past.SetActive(eve.to == TimeTravelPeriod.Past);
        
        if(present != null)
            present.SetActive(eve.to == TimeTravelPeriod.Present);
    }

    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel);
    }
}
