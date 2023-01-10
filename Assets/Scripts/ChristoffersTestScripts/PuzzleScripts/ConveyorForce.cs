using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class ConveyorForce : MonoBehaviour {
    [SerializeField] private float standardSpeed;
    [SerializeField] private float minDistanceToWayPoint = 0.1f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer meshRenderer2;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Dictionary<int, float> wayPointSpeeds = new Dictionary<int, float>();
    [SerializeField] private wayPointSpeedInfo[] speedInfo;
    [SerializeField] private float cubeMaxHeigt;
    private MaterialPropertyBlock _matPropBlock;
    private AudioSource audioSource;


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
        _matPropBlock = new MaterialPropertyBlock();
        audioSource = GetComponent<AudioSource>();
        PauseMenuBehaviour.pauseDelegate += TurnOff;
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

                float tempSpeed = standardSpeed;
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


    public void TurnOff(bool turnOff = true) {
        if (!turnOff) return;
        isOn = false;
        audioSource.Stop();
        if (_matPropBlock != null) {
            _matPropBlock.SetFloat("_Scrolling_Time_X", 0f);

            // Apply the edited values to the renderer.
            meshRenderer.SetPropertyBlock(_matPropBlock);
        }
        if (_matPropBlock != null) {
            _matPropBlock.SetFloat("_Scrolling_Time_X", 0f);

            // Apply the edited values to the renderer.
            meshRenderer2.SetPropertyBlock(_matPropBlock);
        }
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
        PauseMenuBehaviour.pauseDelegate -= TurnOff;
    }


}
