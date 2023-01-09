using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAndEffectManager : MonoBehaviour
{
    private Light[] lightsInRoom;
    private ParticleSystem[] particlesInRoom;
    private static ReflectionProbe[] probesInRoom;

    void Start()
    {
        lightsInRoom = GetComponentsInChildren<Light>(true);
        particlesInRoom = GetComponentsInChildren<ParticleSystem>(true);
        probesInRoom = FindObjectsOfType<ReflectionProbe>(true);

        ActivateLights(false);
        ActivateParticles(false);
        ReloadProbes();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateLights(true);
            ActivateParticles(true);
            ReloadProbes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateLights(false);
            ActivateParticles(false);
            ReloadProbes();
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
                else if (!activate && particlesInRoom[i].isPlaying)
                {
                    particlesInRoom[i].Pause();
                }
            }
        }
    }

    private void ReloadProbes()
    {
        if (probesInRoom != null && probesInRoom.Length > 0)
        {
            for (int i = 0; i < probesInRoom.Length; i++)
            {
                if (probesInRoom[i] != null)
                {
                    probesInRoom[i].RenderProbe();
                }
            }
        }
    }
}
