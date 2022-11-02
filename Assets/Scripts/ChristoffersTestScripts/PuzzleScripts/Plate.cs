using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private Material OnMaterial;
    [SerializeField] private Material OffMaterial;

    [SerializeField] private bool isPast;
    [SerializeField] private bool isPresent;
    [SerializeField] private bool isFuture;

    [SerializeField] private GameObject cord;
    [SerializeField] private GameObject cord2;
    [SerializeField] private GameObject sign;

    public bool pastOn;
    public bool presentOn;
    public bool futureOn;

    private MeshRenderer meshRenderer;
    private MeshRenderer cordMeshRenderer;
    private MeshRenderer cord2MeshRenderer;
    private MeshRenderer signMeshRenderer;


    private void Awake()
    {
        pastOn = false;
        presentOn = false;
        futureOn = false;

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        cordMeshRenderer = cord.GetComponent<MeshRenderer>();
        cord2MeshRenderer = cord2.GetComponent<MeshRenderer>();
        signMeshRenderer = sign.GetComponent<MeshRenderer>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (isPast)
        {
            if (other.gameObject.tag == "CubePast")
            {
                meshRenderer.material = OnMaterial;
                //gameObject.GetComponentInChildren<MeshRenderer>().material = OnMaterial;
                cordMeshRenderer.material = OnMaterial;
                cord2MeshRenderer.material = OnMaterial;
                signMeshRenderer.material = OnMaterial;
                pastOn = true; 
            }
        }
        if (isPresent)
        {
            if (other.gameObject.tag == "CubePresent")
            {
                meshRenderer.material = OnMaterial;
                //gameObject.GetComponentInChildren<MeshRenderer>().material = OnMaterial;
                cordMeshRenderer.material = OnMaterial;
                cord2MeshRenderer.material = OnMaterial;
                signMeshRenderer.material = OnMaterial;
                presentOn = true; 
            }
        }
        if (isFuture)
        {
            if (other.gameObject.tag == "CubeFuture")
            {
                meshRenderer.material = OnMaterial;
                //gameObject.GetComponentInChildren<MeshRenderer>().material = OnMaterial;
                cordMeshRenderer.material = OnMaterial;
                cord2MeshRenderer.material = OnMaterial;
                signMeshRenderer.material = OnMaterial;
                futureOn = true; 
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isPast)
        {
            if (other.gameObject.tag == "CubePast")
            {
                meshRenderer.material = OffMaterial;
                cordMeshRenderer.material = OffMaterial;
                cord2MeshRenderer.material = OffMaterial;
                signMeshRenderer.material = OffMaterial;
                //gameObject.GetComponentInChildren<MeshRenderer>().material = OffMaterial;
                pastOn = false;
            }
        }
        if (isPresent)
        {
            if (other.gameObject.tag == "CubePresent")
            {
                meshRenderer.material = OffMaterial;
                cordMeshRenderer.material = OffMaterial;
                cord2MeshRenderer.material = OffMaterial;
                signMeshRenderer.material = OffMaterial;
                //gameObject.GetComponentInChildren<MeshRenderer>().material = OffMaterial;
                presentOn = false;
            }
        }
        if (isFuture)
        {
            if (other.gameObject.tag == "CubeFuture")
            {
                meshRenderer.material = OffMaterial;
                cordMeshRenderer.material = OffMaterial;
                cord2MeshRenderer.material = OffMaterial;
                signMeshRenderer.material = OffMaterial;
                //gameObject.GetComponentInChildren<MeshRenderer>().material = OffMaterial;
                futureOn = false;
            }
        }
    }


}
