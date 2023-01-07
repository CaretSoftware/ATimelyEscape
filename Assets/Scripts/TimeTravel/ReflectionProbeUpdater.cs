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
        _probe.RenderProbe();
        TimePeriodChanged.AddListener<TimePeriodChanged>(TriggerTimer);
    }

    private void TriggerTimer(TimePeriodChanged e)
    {
        if (e.IsReload) return;
        UpdateProbe();
        Invoke(nameof(UpdateProbe), 1f);
    }

    private void UpdateProbe()
    {
        _probe.RenderProbe();
    }

    private void OnDestroy() {
        if(EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TriggerTimer);
    }
}
