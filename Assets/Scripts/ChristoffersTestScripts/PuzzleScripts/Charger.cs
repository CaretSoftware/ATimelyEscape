using CallbackSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
    [SerializeField] private int pastCharge = 2;
    [SerializeField] private int presentCharge = 1;
    [SerializeField] private int futerCharge = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        CubeCharge cubeCharge= other.GetComponent<CubeCharge>();
        if (cubeCharge != null)
        {
            if (cubeCharge.pastCubeCharge != null && cubeCharge.pastCubeCharge.pastCubeCharge != null) { cubeCharge.Charging(futerCharge, this); }
            else if (cubeCharge.pastCubeCharge != null) { cubeCharge.Charging(presentCharge, this); }
            else cubeCharge.Charging(pastCharge, this);
        }
    }
}
