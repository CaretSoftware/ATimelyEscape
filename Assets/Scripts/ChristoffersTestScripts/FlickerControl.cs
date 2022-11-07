using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerControl : MonoBehaviour
{
    [SerializeField] private bool isFlickering;
    [Range(0.02f, 0.5f)]
    [SerializeField] private float flickDelay;
    private float timeDelay;

    void Update()
    {
        if (!isFlickering)
        {
            StartCoroutine(FlickeringLight());
        }
    }
    IEnumerator FlickeringLight()
    {
        isFlickering = true;
        this.gameObject.GetComponent<Light>().enabled = false;
        timeDelay = Random.Range(0.01f, flickDelay);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light>().enabled = true;
        timeDelay = Random.Range(0.01f, flickDelay);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false; 
    }
}
