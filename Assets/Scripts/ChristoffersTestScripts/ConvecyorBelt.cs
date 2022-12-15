using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvecyorBelt : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnThis;
    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private Transform spawnPos;
    private bool isOn;

    private void Awake()
    {
        isOn = true; 
    }
    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        while (true && isOn)
        {
            int randomIndex = Random.Range(0, spawnThis.Length);

            Instantiate(spawnThis[randomIndex], spawnPos.position, spawnPos.rotation);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    public void mashineOff()
    {
        isOn = false;
    }
}
