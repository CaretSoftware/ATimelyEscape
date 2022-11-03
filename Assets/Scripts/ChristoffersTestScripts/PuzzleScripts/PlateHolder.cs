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

    private Plate pastPlate;
    private Plate presentPlate;
    private Plate futurePlate;

    private void Awake()
    {
        pastPlate = past.GetComponent<Plate>();
        presentPlate = present.GetComponent<Plate>();
        futurePlate = future.GetComponent<Plate>();
    }
    private void Update()
    {
        if (pastPlate.pastOn && presentPlate.presentOn && futurePlate.futureOn)
        {
            hatch.GetComponent<Animator>().SetBool("Open", true);
            candyFeeder.GetComponent<Animator>().SetBool("Open", true);
        }
    }
}
