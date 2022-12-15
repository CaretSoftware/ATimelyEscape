using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotController : MonoBehaviour
{

    public AudioMixerSnapshot past;
    public AudioMixerSnapshot present;
    public AudioMixerSnapshot future;

    private AudioSource source;
    public AudioClip zap;

    // Start is called before the first frame update
    void Start()
    {
        present.TransitionTo(0f);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            past.TransitionTo(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            present.TransitionTo(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            future.TransitionTo(0.5f);
        }
    }

}