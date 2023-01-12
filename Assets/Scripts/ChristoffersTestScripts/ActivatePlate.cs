using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlate : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Start()
    {
        meshRenderer.enabled = false;
    }

    public void PlateActivate()
    {
        meshRenderer.enabled = true; 
    }


}
