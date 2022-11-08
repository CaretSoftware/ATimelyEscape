using CallbackSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
    [SerializeField] private int charge = 1;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Cube")
        {
            other.gameObject.GetComponent<CubeCharge>().Charging(charge);
        }
    }
}
