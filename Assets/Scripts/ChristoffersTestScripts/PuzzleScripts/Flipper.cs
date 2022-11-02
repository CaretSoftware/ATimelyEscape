using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    [SerializeField] private float flipForceZ = 1000.0f;
    private Rigidbody rb;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            rb = other.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(-transform.forward * flipForceZ);
            rb.AddForce(-transform.up * flipForceZ);
            rb.useGravity = true; 

        }
    }
}
