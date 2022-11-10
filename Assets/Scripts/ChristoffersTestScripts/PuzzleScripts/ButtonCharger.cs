using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCharger : MonoBehaviour
{
    [SerializeField] private GameObject chargerON;
    [SerializeField] private GameObject chargerOFF;
    [SerializeField] private LayerMask layerMask;
    private int objectsOnButton;

    // Start is called before the first frame update
    void Start()
    {
        chargerON.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | 1 << other.gameObject.layer))
        {
            objectsOnButton++;
            chargerON.SetActive(true);
            chargerOFF.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | 1 << other.gameObject.layer))
        {
            objectsOnButton--;
            if (objectsOnButton == 0)
            {
                chargerON.SetActive(false);
                chargerOFF.SetActive(true);
            }
        }
    }
}

   
