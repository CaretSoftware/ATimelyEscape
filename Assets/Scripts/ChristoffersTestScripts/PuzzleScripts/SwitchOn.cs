using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchOn : MonoBehaviour
{
    [SerializeField] private UnityEvent whatToDo;


    private void OnTriggerEnter(Collider other)
    {
        if (whatToDo != null && other.gameObject.tag == "Cube")
        {
            whatToDo.Invoke();
        }
    }

}
