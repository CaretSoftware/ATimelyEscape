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
        if (_probe.enabled)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateProbeWithTime());
        }
    }

    private void TriggerTimer(TimePeriodChanged e)
    {
        if (e.IsReload) return;
        UpdateProbe();
    }

    private IEnumerator UpdateProbeWithTime()
    {
        _probe.RenderProbe(null);
        yield return new WaitForSecondsRealtime(0.5f);
        _probe.RenderProbe(null);
    }

    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TriggerTimer);
    }
}
