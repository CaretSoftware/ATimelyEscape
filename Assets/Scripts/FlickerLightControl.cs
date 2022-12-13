using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLightControl : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Values")]
    public bool isFlickering = false;
    public float timeDelay;
    public int flickeringDivision = 2;

    [Header("Lights")]
    public Light spotLight;
    public Light pointLight;

    private float originalSpotIntensity;
    private float originalPointIntensity;
    private AudioSource audioSource; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(spotLight == null)
            spotLight = GetComponent<Light>();
        originalSpotIntensity = spotLight.intensity;

        if (pointLight == null)
            pointLight = GetComponentInChildren<Light>();
        originalPointIntensity = pointLight.intensity;
    }

    void Update()
    {
        if (!isFlickering)
        {
            StartCoroutine(FlickeringLight());
            audioSource.Play();
        }
    }
     
    IEnumerator FlickeringLight()
    {
        isFlickering = true;
        spotLight.intensity = spotLight.intensity / flickeringDivision;
        pointLight.intensity = pointLight.intensity / flickeringDivision;
        timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(timeDelay);

        spotLight.intensity = originalSpotIntensity;
        pointLight.intensity = originalPointIntensity;
        timeDelay = Random.Range(0.1f, 2f);
        yield return new WaitForSeconds(timeDelay);

        isFlickering = false;
    }
}
