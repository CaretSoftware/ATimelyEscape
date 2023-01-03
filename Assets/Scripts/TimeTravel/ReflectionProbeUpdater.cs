using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;


public class ReflectionProbeUpdater : MonoBehaviour
{
    private ReflectionProbe probe;

    void Start()
    {
        probe = GetComponent<ReflectionProbe>();
        probe.RenderProbe();
        TimePeriodChanged.AddListener<TimePeriodChanged>(UpdateProbe);
    }

    private void UpdateProbe(TimePeriodChanged e)
    {
        if (e.IsReload) return;
        probe.RenderProbe();
    }
}
