using System.Collections;
using System.Collections.Generic;
using RatCharacterController;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TechnoVacuum : MonoBehaviour {
    // Start is called before the first frame update

    private Transform playerTransform;
    [SerializeField] AudioSource source;
    [SerializeField] private AudioReverbFilter reverbFilter;
    [SerializeField] private AudioClip music;
    [SerializeField] private float minDistance;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private GameObject light;
    private float bpm = 139;

    void Start() {
        playerTransform = FindObjectOfType<CharacterAnimationController>().transform;
        /*bpm = UniBpmAnalyzer.AnalyzeBpm(music);*/
    }

    // Update is called once per frame
    void Update() {
        /*light.SetActive(false);*/
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= minDistance && !source.isPlaying) {
            source.clip = music;
            source.time = 54f;
            source.Play();
            source.volume = Mathf.Lerp(1, 0.01f, curve.Evaluate(dist / minDistance));
            reverbFilter.reverbLevel = Mathf.SmoothStep(-200, 450, dist / minDistance);
            if (!isRunning) StartCoroutine(LightShow());
        } else if (dist <= minDistance && source.isPlaying) {
            source.volume = Mathf.Lerp(1, 0.01f, curve.Evaluate(dist / minDistance));
            reverbFilter.reverbLevel = Mathf.SmoothStep(-200, 450, dist / minDistance);
            if (!isRunning) StartCoroutine(LightShow());
        } /*else if (dist >= minDistance && source.isPlaying) source.Stop();*/
    }

    private bool isRunning;

    private IEnumerator LightShow() {
        isRunning = true;
        light.SetActive(false);
        yield return new WaitForSecondsRealtime(60f / bpm);
        light.SetActive(true);
        isRunning = false;
    }
}