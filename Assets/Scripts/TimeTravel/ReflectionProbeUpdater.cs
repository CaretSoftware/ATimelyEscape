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
        // Start probe update
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            _probe.RenderProbe(null);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TriggerTimer);
    }
}
