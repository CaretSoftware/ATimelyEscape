using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class ConveyorForce : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minDistanceToWayPoint = 0.1f;
    [SerializeField] private float cubeSpeed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer meshRenderer2;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Dictionary<int, float> wayPointSpeeds = new Dictionary<int, float>();
    [SerializeField] private wayPointSpeedInfo[] speedInfo;
    [SerializeField] private float cubeMaxHeigt;
    private MaterialPropertyBlock _matPropBlock;
    private AudioSource audioSource;

    [Serializable]
    private class wayPointSpeedInfo
    {
        public int index;
        public float speed;
    }

    private HashSet<GameObject> removedCubes = new HashSet<GameObject>();

    private Dictionary<GameObject, int> cubeDict = new Dictionary<GameObject, int>();

    private bool isOn;

    void Awake()
    {
        foreach (var info in speedInfo)
        {
            wayPointSpeeds.Add(info.index, info.speed);
        }
        _matPropBlock = new MaterialPropertyBlock();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        isOn = false;
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeMachineOff);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            if (!cubeDict.ContainsKey(other.gameObject))
                cubeDict.Add(other.gameObject, 0);
        }
    }

    private void Update()
    {
        if (isOn)
        {

            foreach (var cube in cubeDict)
            {
                if (cube.Key == null || cube.Key.transform.position.y < cubeMaxHeigt)
                {
                    removedCubes.Add(cube.Key);
                    continue;
                }

                float wayPointSpeed = cubeSpeed;

                if (wayPointSpeeds.ContainsKey(cubeDict[cube.Key])) wayPointSpeed = wayPointSpeeds[cubeDict[cube.Key]];

                //print("befoe " + cube.Key.transform.position);
                cube.Key.transform.position = Vector3.MoveTowards(cube.Key.transform.position, waypoints[cube.Value].position, wayPointSpeed);
                if (Vector3.Distance(cube.Key.transform.position, waypoints[cube.Value].position) < minDistanceToWayPoint) cubeDict[cube.Key] += 1;
                //print("after " + cube.Key.transform.position);
            }

            foreach (var cube in removedCubes)
            {
                cubeDict.Remove(cube);
            }
            removedCubes.Clear();

        }

    }


    public void TurnOff()
    {
        isOn = false;
        audioSource.Stop();
        if (_matPropBlock != null)
        {
            _matPropBlock.SetFloat("_Scrolling_Time_X", 0f);

            // Apply the edited values to the renderer.
            meshRenderer.SetPropertyBlock(_matPropBlock);
        }
        if (_matPropBlock != null)
        {
            _matPropBlock.SetFloat("_Scrolling_Time_X", 0f);

            // Apply the edited values to the renderer.
            meshRenderer2.SetPropertyBlock(_matPropBlock);
        }
    }
    private void TimeMachineOff(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Past)
        {
            isOn = false;
            audioSource.Stop();
        }

        else if (e.to == TimeTravelPeriod.Past)
        {
            isOn = true;
            audioSource.Play();
        }

    }
    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeMachineOff);
    }


}
