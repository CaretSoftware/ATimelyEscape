using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class AnimationCount : MonoBehaviour {
    [SerializeField] private string animationName;
    int numHookAnimations = 5;
    private Animator[] animators;
    private Rigidbody[] rigidbodies;
    private void Start() {
        animators = GetComponentsInChildren<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        if (TimeTravelManager.currentPeriod == TimeTravelPeriod.Past) {
            for (int i = 0; i < animators.Length; ++i) {
                animators[i].Play(animationName, 0, 1.0f / numHookAnimations * i);
            }
        }
        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimeTravel);
    }

    private void OnTimeTravel(TimePeriodChanged e) {
        if (e.to != TimeTravelPeriod.Past) {
            for (int i = 0; i < animators.Length; ++i) {
                animators[i].StopPlayback();
                animators[i].enabled = false;
            }
        } else {
            for (int i = 0; i < animators.Length; ++i) {
                animators[i].enabled = true;
                animators[i].Play(animationName, 0, 1.0f / numHookAnimations * i);
            }
        }
    }

    private void OnDestroy() {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(OnTimeTravel);
    }
}
