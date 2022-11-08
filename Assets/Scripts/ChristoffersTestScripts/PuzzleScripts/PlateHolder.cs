using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateHolder : MonoBehaviour
{
    [SerializeField] private GameObject hatch;
    [SerializeField] private GameObject candyFeeder;
    [SerializeField] private GameObject past;
    [SerializeField] private GameObject present;
    [SerializeField] private GameObject future;
    [SerializeField] private GameObject futureTimer;
    [SerializeField] private GameObject doneCord;
    [SerializeField] private GameObject doneCord2;
    [SerializeField] private GameObject doneCord3;
    [SerializeField] private Material onMaterial;
    [SerializeField] private GameObject wellDoneText;


    private Plate pastPlate;
    private Plate presentPlate;
    private Plate futurePlate;
    private DigitalClock futureTimerClock;

    public bool allIsDone; 

    private void Awake()
    {
        doneCord.SetActive(false);
        doneCord2.SetActive(false);
        doneCord3.SetActive(false);
        wellDoneText.SetActive(false);
        allIsDone = false;
        pastPlate = past.GetComponent<Plate>();
        presentPlate = present.GetComponent<Plate>();
        futurePlate = future.GetComponent<Plate>();
        futureTimerClock = futureTimer.GetComponent<DigitalClock>();
    }
    private void Update()
    {
        if (pastPlate.pastOn && presentPlate.presentOn && futurePlate.futureOn && futureTimerClock.futureDone)
        {
            hatch.GetComponent<Animator>().SetBool("Open", true);
            candyFeeder.GetComponent<Animator>().SetBool("Open", true);
            pastPlate.GetComponent<MeshRenderer>().material = onMaterial;
            presentPlate.GetComponent<MeshRenderer>().material = onMaterial;
            futurePlate.GetComponent<MeshRenderer>().material = onMaterial;
            doneCord.SetActive(true);
            doneCord2.SetActive(true);
            doneCord3.SetActive(true);
            allIsDone = true;
            wellDoneText.SetActive(true);
        }
    }
}
