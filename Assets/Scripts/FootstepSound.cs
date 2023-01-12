using System;
using System.Collections;
using System.Collections.Generic;
using NewRatCharacterController;
using UnityEngine;
using Random = UnityEngine.Random;

public class FootstepSound : MonoBehaviour {
    [SerializeField, Range(0f, 1.0f)] private float footStepVolume = .5f; 
    [SerializeField, Range(0f, 1.0f)] private float randomVolumePercentage = .25f;
    [SerializeField] private AudioClip[] footSteps;
    
    private NewRatCharacterController.NewRatCharacterController _characterController;
    
    private float animationWeightThreshold = .5f;
    private float maxVelocity = 1.0f;
    private float minVelocity = .5f;
    private float _currentVelocity = 1.0f;

    private AudioSource _audioSource;

    private void Start() {
        _audioSource = GetComponentInChildren<AudioSource>();
        _characterController = GetComponent<NewRatCharacterController.NewRatCharacterController>();
    }

    public void Step(AnimationEvent evt) {
        if (evt.animatorClipInfo.weight > animationWeightThreshold) {
            if (_characterController != null)
                _currentVelocity = _characterController._velocity.magnitude;
            float randomVolumePercentage = 1f - Random.Range(0f, this.randomVolumePercentage);
            float volume = Mathf.InverseLerp(minVelocity,   maxVelocity, _currentVelocity) * footStepVolume * randomVolumePercentage;
            PlayFootstep(volume);
        }
    }

    [ContextMenu("Play Footstep")]
    private void PlayFootstep() => PlayFootstep(1.0f); 
    private void PlayFootstep(float volume) {
        int rnd = Random.Range(0, footSteps.Length);
        _audioSource.clip = footSteps[rnd];
        _audioSource.PlayOneShot(footSteps[rnd], volume);
        // PlayClipAtPoint(footSteps[rnd], transform.position, volume);
    }
}
