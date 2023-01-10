using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchOn : MonoBehaviour
{

    [SerializeField] private bool isInteractableByPlayer;
    [SerializeField] private bool isTreePuzzle;
    /*    [SerializeField] private bool isInteractableByPast;
        [SerializeField] private bool isInteractableByPresent;*/
    [SerializeField] private UnityEvent switchOn;
    [SerializeField] private UnityEvent switchOff;
    private Material onMaterial;
    private Material offMaterial;
    private MeshRenderer meshRenderer;
    private bool isOn;
    private Animator animator;
    private void Start()
    {
        onMaterial = Resources.Load("TestButtonOn") as Material;
        offMaterial = Resources.Load("M_CB") as Material;
        meshRenderer = GetComponent<MeshRenderer>();
        isOn = true;
        if (isInteractableByPlayer)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (switchOn != null && !isInteractableByPlayer && other.gameObject.tag == "Cube")
        {
            if (isOn)
            {
                switchOn.Invoke();
                if (!isTreePuzzle)
                {
                    meshRenderer.material = onMaterial;
                }
            }
        }
        else if (switchOn != null && isInteractableByPlayer && other.gameObject.tag == "Player")
        {
            if (isOn)
            {
                switchOn.Invoke();
                if (animator != null)
                {
                    animator.SetBool("On", true);
                }
            }
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
            if (isOn)
            {
                switchOff.Invoke();
                if (!isTreePuzzle)
                {
                    meshRenderer.material = offMaterial;
                }
            }
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
    public void ButtonOff()
    {
        isOn = false;
    }
    public void ButtonOn()
    {
        isOn = true;
    }

}
