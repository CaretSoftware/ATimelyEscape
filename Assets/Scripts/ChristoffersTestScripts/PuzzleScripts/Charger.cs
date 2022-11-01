using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
    [SerializeField] private Material chargedMaterial;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Cube")
        {
            other.gameObject.GetComponent<CubePush>().enabled = true;
            other.gameObject.GetComponent<MeshRenderer>().material = chargedMaterial;
            Transform uI = other.gameObject.transform.GetChild(0);
            uI.transform.gameObject.SetActive(true);


        }
    }
}
