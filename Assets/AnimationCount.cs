using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCount : MonoBehaviour
{
    [SerializeField] private string animationName; 
    int numHookAnimations = 5;
    private Animator[] animators;
    private void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        for (int i = 0; i < animators.Length; ++i)
        {
            animators[i].Play(animationName, 0, 1.0f / numHookAnimations * i);
        }
    }
}
