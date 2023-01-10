using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeColor : MonoBehaviour
{
    [SerializeField] private LayerMask cubeLayerMask;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private GameObject crosshairRed;
    private Camera cam;
    private Ray ray;
    
    private void Start() {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        RaycastHit hit;
        RaycastHit keyPadHit;
        //Ray ray = new Ray(transform.position, transform.forward);
        ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        bool cubeHit = Physics.Raycast(ray, out hit, 10, cubeLayerMask, QueryTriggerInteraction.Ignore);
        bool keypadHit = Physics.Raycast(ray, out keyPadHit, 10, defaultLayerMask, QueryTriggerInteraction.Ignore) 
                          && keyPadHit.collider.CompareTag("Keypad");
        
        bool hitSomething = cubeHit || keypadHit;
        
        crosshairRed.SetActive(hitSomething);
    }
}
