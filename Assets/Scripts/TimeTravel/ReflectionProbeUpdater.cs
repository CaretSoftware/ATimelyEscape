using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;


public class ReflectionProbeUpdater : MonoBehaviour
{
    private ReflectionProbe _probe;

    void Start()
    {
        _probe = GetComponent<ReflectionProbe>();
        UpdateProbe();
        TimePeriodChanged.AddListener<TimePeriodChanged>(TriggerTimer);
    }

    public void UpdateProbe()
    {
        if(_probe.enabled) { 
            _probe.RenderProbe(null);
        }
    }

    private void TriggerTimer(TimePeriodChanged e)
    {
        if (e.IsReload) return;
        UpdateProbe();
        Invoke(nameof(UpdateProbe), 6f * Time.deltaTime);
    }

    private void OnDestroy() {
        if(EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TriggerTimer);
    }
}
