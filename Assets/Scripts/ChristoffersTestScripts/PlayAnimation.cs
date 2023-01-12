using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    void Start()
    {
        animator = GetComponent<Animator>();
        if(GetComponent<AudioSource>() != null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public void StartAnimation()
    {
        animator.SetBool("On", true);
        if(audioSource != null)
        {
            audioSource.Play();
        }
    }


}
