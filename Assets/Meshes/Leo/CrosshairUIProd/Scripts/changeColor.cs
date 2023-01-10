using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeColor : MonoBehaviour
{
    [SerializeField] private LayerMask cubeLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private GameObject crosshairRed;
    void Start()
    {
     
        
    }

    void Update()
    {
        RaycastHit hit;
        RaycastHit keyPadHit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 10, cubeLayerMask) ||
            (Physics.Raycast(transform.position, transform.forward, out keyPadHit, 10,defaultLayerMask, QueryTriggerInteraction.Ignore) && keyPadHit.collider.CompareTag("Keypad")))
        {
            Debug.Log(true);
            crosshairRed.SetActive(true);
        }
        else
        {
            Debug.Log(false);
            crosshairRed.SetActive(false);
        }

    }
}
