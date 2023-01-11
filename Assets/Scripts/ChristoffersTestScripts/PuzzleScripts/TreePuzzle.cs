using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class TreePuzzle : MonoBehaviour
{
    [SerializeReference] private GameObject teleporter;
    private bool isOn;

    void Start()
    {
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
        teleporter.SetActive(false);
    }

    public void TeleportOn()
    {
        isOn = true;
    }
    private void TimeTravel(TimePeriodChanged e)
    {
        if(isOn)
        {
            if (e.from == TimeTravelPeriod.Future)
            {
                teleporter.SetActive(false);
            }
            if(e.to == TimeTravelPeriod.Future)
            {
                teleporter.SetActive(true);
            }
        }

    }
    private void OnDestroy() { if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel); }

}
