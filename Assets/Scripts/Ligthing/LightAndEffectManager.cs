using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAndEffectManager : MonoBehaviour
{
    private Light[] lightsInRoom;
    private ParticleSystem[] particlesInRoom;

    void Start()
    {
        lightsInRoom = GetComponentsInChildren<Light>();
        particlesInRoom = GetComponentsInChildren<ParticleSystem>();

        ActivateLights(false);
        ActivateParticles(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateLights(true);
            ActivateParticles(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateLights(false);
            ActivateParticles(false);
        }
    }

    private void ActivateLights(bool activate)
    {
        if (lightsInRoom != null && lightsInRoom.Length > 0)
        {
            for (int i = 0; i < lightsInRoom.Length; i++)
            {
                lightsInRoom[i].enabled = activate;
            }
        }
    }

    private void ActivateParticles(bool activate)
    {
        if (particlesInRoom != null && particlesInRoom.Length > 0)
        {
            for (int i = 0; i < particlesInRoom.Length; i++)
            {
                if (activate && particlesInRoom[i].isPaused)
                {
                    particlesInRoom[i].Play();
                }
                else if(!activate && particlesInRoom[i].isPlaying)
                {
                    particlesInRoom[i].Pause();
                }
            }
        }
    }
}
