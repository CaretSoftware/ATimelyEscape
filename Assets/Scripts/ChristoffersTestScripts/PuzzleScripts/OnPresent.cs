using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPresent : MonoBehaviour
{
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private UnityEvent switchOn;
    [SerializeField] private UnityEvent switchOff;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (switchOn != null && other.gameObject.tag == "CubePast")
        {
            switchOn.Invoke();
            meshRenderer.material = onMaterial;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (switchOff != null && other.gameObject.tag == "CubePast")
        {
            switchOff.Invoke();
            meshRenderer.material = offMaterial;
        }
    }
}
