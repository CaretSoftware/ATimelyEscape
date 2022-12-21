using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBridge : MonoBehaviour
{
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject bridge2;

    private void Awake()
    {
        bridge.SetActive(false);
        if(bridge2 != null)
        {
            bridge2.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            bridge.SetActive(true);
            if (bridge2 != null)
            {
                bridge2.SetActive(true);
            } else if(other.gameObject.tag == "CubePast")
            {
                bridge.SetActive(true);
                if (bridge2 != null) {
                    bridge2.SetActive(true);
                }
            }
            else if (other.gameObject.tag == "CubePresent") {
                bridge.SetActive(true);
                if (bridge2 != null) {
                    bridge2.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            bridge.SetActive(false);
            if (bridge2 != null)
            {
                bridge2.SetActive(false);
            }
        }
    }
}
