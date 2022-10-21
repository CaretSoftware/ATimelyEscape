using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class TimeTravelObjectManager : MonoBehaviour {
    [SerializeField] private TimeTravelObject past, present, future;
    [SerializeField] private bool canBeMoved;
    public bool CanBeMoved => canBeMoved;

    // Start is called before the first frame update
    private void Awake() {
        past.SetUpTimeTravelObject(this);
        present.SetUpTimeTravelObject(this, past);
        future.SetUpTimeTravelObject(this, present);
        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimePeriodChanged);
    }

    public void OnTimePeriodChanged(TimePeriodChanged e) {
        past.gameObject.SetActive(e.to == TimeTravelPeriod.Past ? true : false);
        present.gameObject.SetActive(e.to == TimeTravelPeriod.Present ? true : false);
        future.gameObject.SetActive(e.to == TimeTravelPeriod.Future ? true : false);
    }
}