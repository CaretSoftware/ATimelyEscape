using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorForce : MonoBehaviour
{
    [SerializeField] private float speed; 
    private void OnTriggerStay(Collider other)
    {
        other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * speed) * Time.deltaTime, ForceMode.Impulse);
    }
}
