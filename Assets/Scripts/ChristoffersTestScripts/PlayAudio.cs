using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private bool oneOn;
    private bool twoOn;
    private bool hasStarted;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(!hasStarted && oneOn && twoOn)
        {
            audioSource.Play();
            hasStarted = true; 
        }
    }
    public void StartSound()
    {
        audioSource.Play();
    }
    public void OneOn()
    {
        oneOn = true;
    }
    public void TwoOn()
    {
        twoOn = true; 
    }


}
