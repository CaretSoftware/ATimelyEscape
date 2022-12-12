using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchOn : MonoBehaviour
{
    [SerializeField] private UnityEvent switchOn;
    [SerializeField] private UnityEvent switchOff;

    private void OnTriggerEnter(Collider other)
    {
        if (switchOn != null && other.gameObject.tag == "Cube")
        {
            switchOn.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (switchOff != null && other.gameObject.tag == "Cube")
        {
            switchOff.Invoke();
        }
    }

}
