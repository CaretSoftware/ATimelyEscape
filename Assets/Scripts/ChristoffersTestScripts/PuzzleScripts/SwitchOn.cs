using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchOn : MonoBehaviour
{

    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private bool isInteractableByPlayer;
    [SerializeField] private bool isInteractableByPast;
    [SerializeField] private bool isInteractableByPresent;
    [SerializeField] private UnityEvent switchOn;
    [SerializeField] private UnityEvent switchOff;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (switchOn != null && other.gameObject.tag == "Cube")
        {
            switchOn.Invoke();
            meshRenderer.material = onMaterial;
        } else if(switchOn != null && isInteractableByPlayer && other.gameObject.tag == "Player")
        {
            switchOn.Invoke();
            meshRenderer.material = onMaterial;
        }
/*        else if (switchOn != null && isInteractableByPast && other.gameObject.tag == "CubePast")
        {
            switchOn.Invoke();
            meshRenderer.material = onMaterial;
        }
        else if (switchOn != null && isInteractableByPresent && other.gameObject.tag == "CubePresent")
        {
            switchOn.Invoke();
            meshRenderer.material = onMaterial;
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (switchOff != null && other.gameObject.tag == "Cube")
        {
            switchOff.Invoke();
            meshRenderer.material = offMaterial;
        }
/*        else if (switchOn != null && isInteractableByPast && other.gameObject.tag == "CubePast")
        {
            switchOff.Invoke();
            meshRenderer.material = offMaterial;
        }
        else if (switchOn != null && isInteractableByPresent && other.gameObject.tag == "CubePresent")
        {
            switchOff.Invoke();
            meshRenderer.material = offMaterial;
        }*/
    }

}
