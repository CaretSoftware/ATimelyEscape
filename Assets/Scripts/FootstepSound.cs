using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] footSteps;
    private AudioSource audioSource;
   
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep(Vector3 position)
    {
        int footstep = Random.Range(0, footSteps.Length);

        AudioSource.PlayClipAtPoint(footSteps[footstep], position);

    }
    
}
