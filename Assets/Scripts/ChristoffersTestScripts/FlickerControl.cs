using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerControl : MonoBehaviour
{
    [SerializeField] private bool isFlickering;
    [Range(0.02f, 3.0f)]
    [SerializeField] private float flickDelay;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    private float timeDelay;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponentInParent<MeshRenderer>();
    }

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
        meshRenderer.material = offMaterial;
        timeDelay = Random.Range(0.01f, flickDelay);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light>().enabled = true;
        meshRenderer.material = onMaterial;
        timeDelay = Random.Range(0.01f, flickDelay);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false; 
    }
}
