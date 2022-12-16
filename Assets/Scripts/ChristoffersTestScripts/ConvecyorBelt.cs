using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class ConvecyorBelt : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnThis;
    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private Transform spawnPos;
    private GameObject[] liveObjects; 
    private bool isOn;

    private void Awake()
    {
        isOn = true;
    }
    private void Start()
    {
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeMachineOff);
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            if (isOn)
            {
                int randomIndex = Random.Range(0, spawnThis.Length);

                Instantiate(spawnThis[randomIndex], spawnPos.position, spawnPos.rotation);
                
            }
            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    public void mashineOff()
    {
        isOn = false;
    }
    private void TimeMachineOff(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Past)
        {
            isOn = false;
        }
        else if (e.to == TimeTravelPeriod.Past)
        {
            isOn = true;
        }
    }

}
