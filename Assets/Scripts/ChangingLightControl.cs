using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingLightControl : MonoBehaviour
{
    [Header("Values")]
    public float timeDelay = 3;

    [Header("Lights")]
    public Light pointLight;
    public Light emissionLight;

    private float originalSpotIntensity;
    private float originalEmissionIntensity;
    private AudioSource audioSource;

    private bool isChanging = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (pointLight == null)
            pointLight = GetComponent<Light>();
        originalSpotIntensity = pointLight.intensity;

        if (emissionLight == null)
            emissionLight = GetComponentInChildren<Light>();
        originalEmissionIntensity = emissionLight.intensity;
    }

    void Update()
    {
        if (!isChanging)
        {
            isChanging = true;
            StartCoroutine(ChangingIntensityLight());
        }
    }

    private IEnumerator ChangingIntensityLight()
    {
        float time = 0;

        while (time < timeDelay)
        {
            time = Time.unscaledDeltaTime;
            pointLight.intensity = Mathf.Lerp(originalSpotIntensity, 0, time / timeDelay);
            emissionLight.intensity = Mathf.Lerp(originalEmissionIntensity, 0, time / timeDelay);

            yield return null;
        }

        time = 0;

        while (time < timeDelay)
        {
            time = Time.unscaledDeltaTime;
            pointLight.intensity = Mathf.Lerp(0, originalSpotIntensity, time / timeDelay);
            emissionLight.intensity = Mathf.Lerp(0, originalEmissionIntensity, time / timeDelay);

            yield return null;
        }

        isChanging = false;
    }
}
