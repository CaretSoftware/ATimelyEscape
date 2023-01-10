using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class ConveyorForceTrigger : MonoBehaviour {
    [SerializeField] private float characterSpeed = 25f;
    /*     [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshRenderer meshRenderer2; */
    private NewRatCharacterController.NewRatCharacterController ratCharacter;
    private AudioSource audioSource;
    private MaterialPropertyBlock _matPropBlock;
    private bool isOn;
    private void Awake() {
        audioSource = gameObject.GetComponent<AudioSource>();
        _matPropBlock = new MaterialPropertyBlock();
    }
    // Start is called before the first frame update
    void Start() {
        isOn = true;
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeMachineOff);
        PauseMenuBehaviour.pauseDelegate += OnPause;
    }

    // Update is called once per frame
    void Update() {
        if (ratCharacter == null || !isOn) return;

        ratCharacter.ConveyorForce = transform.forward * characterSpeed * Time.deltaTime;

    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            ratCharacter = other.GetComponent<NewRatCharacterController.NewRatCharacterController>();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            ratCharacter = null;
        }
    }

    private void OnPause(bool paused) {
        if (audioSource != null) {
            if (paused) audioSource.Stop();
            else if (!paused && TimeTravelManager.currentPeriod == TimeTravelPeriod.Past) audioSource.Play();
        }
    }

    private void TimeMachineOff(TimePeriodChanged e) {
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present) {
            isOn = false;
            if (audioSource != null) {
                audioSource.Stop();
            }
        } else if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Future) {
            isOn = false;
            if (audioSource != null) {
                audioSource.Stop();
            }
        } else if (e.from == TimeTravelPeriod.Future && e.to == TimeTravelPeriod.Past) {
            isOn = true;
            if (audioSource != null) {
                audioSource.Play();
            }
        } else if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past) {
            isOn = true;
            if (audioSource != null) {
                audioSource.Play();
            }
        }
    }
    public void TurnOff() { isOn = false; }
    private void OnDestroy() {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeMachineOff);
        PauseMenuBehaviour.pauseDelegate -= OnPause;
    }

}
