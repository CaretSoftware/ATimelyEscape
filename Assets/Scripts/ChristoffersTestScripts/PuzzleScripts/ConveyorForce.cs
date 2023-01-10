using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class ConveyorForce : MonoBehaviour {
    [SerializeField] private float standardConveyorBeltSpeed;
    [SerializeField] private float minDistanceToWayPoint = 0.1f;
    [SerializeField] private float cubeMaxHeigt;
    /*     [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshRenderer meshRenderer2; */
    [SerializeField] private List<MeshRenderer> conveyorBeltRenderers;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Dictionary<int, float> wayPointSpeeds = new Dictionary<int, float>();
    [SerializeField] private wayPointSpeedInfo[] speedInfo;
    private MaterialPropertyBlock matPropBlockSlow, matPropBlockFast;
    private AudioSource audioSource;
    private bool conveyorPermanentlyOff;


    [Serializable]
    private class wayPointSpeedInfo {
        public int index;
        public float speed;
    }

    private HashSet<GameObject> removedCubes = new HashSet<GameObject>();
    private HashSet<GameObject> cubes = new HashSet<GameObject>();
    private Dictionary<GameObject, int> cubeDict = new Dictionary<GameObject, int>();

    private bool isOn;

    void Awake() {
        foreach (var info in speedInfo) {
            wayPointSpeeds.Add(info.index, info.speed);
        }
        matPropBlockSlow = new MaterialPropertyBlock();
        matPropBlockFast = new MaterialPropertyBlock();
        audioSource = GetComponent<AudioSource>();
        PauseMenuBehaviour.pauseDelegate += TurnOffDelegate;
    }

    private void Start() {
        isOn = false;
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeMachineOff);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Cube") {
            if (!cubeDict.ContainsKey(other.gameObject))
                cubeDict.Add(other.gameObject, 0);
            cubes.Add(other.gameObject);
        }
    }

    private void Update() {
        if (isOn) {

            foreach (var cube in cubes) {
                if (cube == null || cube.transform.position.y < cubeMaxHeigt) {
                    removedCubes.Add(cube);
                    continue;
                }

                float tempSpeed = standardConveyorBeltSpeed;
                if (wayPointSpeeds.ContainsKey(cubeDict[cube])) tempSpeed = wayPointSpeeds[cubeDict[cube]];

                cube.transform.position = Vector3.MoveTowards(cube.transform.position, waypoints[cubeDict[cube]].position, tempSpeed * Time.deltaTime);
                if (Vector3.Distance(cube.transform.position, waypoints[cubeDict[cube]].position) < minDistanceToWayPoint && cubeDict[cube] < waypoints.Length - 1) cubeDict[cube] += 1;
            }

            foreach (var cube in removedCubes) {
                cubeDict.Remove(cube);
                cubes.Remove(cube);
            }
            removedCubes.Clear();
        }
    }

    private void TurnOffDelegate(bool paused) { TurnOff(paused, false); }
    public void TurnOffPermanently() { TurnOff(true, true); }
    private void TurnOff(bool turnOff = true, bool finalTurnOff = false) {
        if (conveyorPermanentlyOff) return;
        isOn = turnOff ? false : true;
        if (turnOff) audioSource.Stop();
        else if(!turnOff && TimeTravelManager.currentPeriod == TimeTravelPeriod.Past) audioSource.Play();
        if (matPropBlockSlow != null && matPropBlockFast != null) {
            matPropBlockSlow.SetFloat("_Scrolling_Time_X", turnOff ? 0f : .2f);
            matPropBlockFast.SetFloat("_Scrolling_TIme_X", turnOff ? 0f : .4f);
            foreach (var renderer in conveyorBeltRenderers) {
                if (!(renderer.materials.Length > 1 && renderer.materials[1].name.Contains("Fast")) || finalTurnOff) renderer.SetPropertyBlock(matPropBlockSlow);
                else renderer.SetPropertyBlock(matPropBlockFast);
            }
        }
        if (finalTurnOff) conveyorPermanentlyOff = true;
    }
    private void TimeMachineOff(TimePeriodChanged e) {
        if (e.from == TimeTravelPeriod.Past) {
            isOn = false;
            audioSource.Stop();
        } else if (e.to == TimeTravelPeriod.Past) {
            isOn = true;
            audioSource.Play();
        }

    }
    private void OnDestroy() {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeMachineOff);
        PauseMenuBehaviour.pauseDelegate -= TurnOffDelegate;
    }


}
