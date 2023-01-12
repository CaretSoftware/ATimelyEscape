using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class SnapshotController : MonoBehaviour
{
    public AudioMixerSnapshot past;
    public AudioMixerSnapshot present;
    public AudioMixerSnapshot future;

    public AudioSource source;
    public AudioClip zap;

    // Start is called before the first frame update
    void Start()
    {
        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimeTravel);
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTimeTravel(TimePeriodChanged tpc)
    {
        if (tpc.IsReload) return;
        source.PlayOneShot(zap);

        switch (tpc.to)
        {
            case TimeTravelPeriod.Past: past.TransitionTo(0.5f); 
                break;
            case TimeTravelPeriod.Present: present.TransitionTo(0.5f);
                break;
            case TimeTravelPeriod.Future: future.TransitionTo(0.5f);
                break;
        }
    }
}